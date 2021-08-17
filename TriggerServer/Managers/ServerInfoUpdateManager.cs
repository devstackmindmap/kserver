using AkaEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using CommonProtocol;
using AkaSerializer;
using AkaConfig;
using StackExchange.Redis;
using Network;
using System.Threading;
using AkaRedisLogic;

namespace TriggerServer.Managers
{
    class ServerInfoUpdateManager
    {
        private string _jobName;
        private string _timerName;
        private ServerList _serverList;
        private RedisKeyType _serverStatusRedisKey;
        private AkaTimer.Timer _checkTimer;
        private List<ServerStateInfo> _managedServers = new List<ServerStateInfo>();
        private IDatabase _redis;
        private object _lockSchedule = new object();
        private object _lockServerList = new object();
        private Server _checkServer;

        public ServerInfoUpdateManager(string jobName, string timerName, ServerList serverList, RedisKeyType serverStatusRedisKey, Server checkServer )
        {
            _checkServer = checkServer;
            _jobName = jobName;
            _timerName = timerName;
            _serverList = serverList;
            _serverStatusRedisKey = serverStatusRedisKey;
        }

        public virtual string GetJobName()
        {
            return _jobName;
        }

        public virtual async Task Initialize()
        {
            _checkTimer = new AkaTimer.Timer(Config.TriggerServerConfig.ServerInfoCheckInterval * 1000 , true, CheckSchedule);
            _checkTimer.Name = _timerName;

            AkaRedis.AkaRedis.AddServer(_checkServer, 
                                        Config.TriggerServerConfig.TriggerRedisSetting.ServerSetting,
                                        Config.TriggerServerConfig.TriggerRedisSetting.Password);

            _redis = AkaRedis.AkaRedis.GetDatabase(_checkServer);
            await RedisInit();
        }

        public virtual void StartScheduler()
        {
            AkaLogger.Logger.Instance().Debug("Server StateInfo  Scheduler Timer Start!");
            _checkTimer.Start();
            ReloadServers();
        }

        public virtual void DoCommand(MessageType msgType, BaseProtocol requestInfo)
        {
            switch (msgType)
            {
                case MessageType.ReloadServerList:
                    AkaLogger.Logger.Instance().Debug($"[ServerInfo] ReloadServer List");
                    ReloadServers();
                    break;
            }
        }

        public virtual void Dispose()
        {
            _checkTimer.DisposeWait();
        }

        private async Task RedisInit()
        {
            //todo set unmanaged all redis
            var maxAreaIndex = _serverList.countryIps.Max(keyValue => keyValue.Key);
            var allServerInfos = await Task.WhenAll( Enumerable.Range(1, maxAreaIndex)
                .Select(areaIndex => ServerStatusInfoJob.GetAllServerStateInfo(_redis, _serverStatusRedisKey, areaIndex)) );

            var serverInfoList = allServerInfos.SelectMany(areaServers => areaServers.values.Select(ip => new ServerStateInfo            {
                Country = areaServers.areaIndex,
                Ip = ip,
                State = ServerStateType.UnManaged,
            })).ToList();

            SetConnectors(serverInfoList);

            foreach (var server in _managedServers)
            {
                Console.WriteLine(" S:" + server.Country + "ip:" + server.Ip);
            }
        }

        private void SetConnectors(List<ServerStateInfo> serverInfoList)
        {
            foreach (var serverStateInfo in serverInfoList)
            {
                var existStateInfo = _managedServers.FirstOrDefault(currentStateInfo => currentStateInfo.Ip == serverStateInfo.Ip && serverStateInfo.Connector != null);
                if (existStateInfo != null)
                {
                    serverStateInfo.Connector = existStateInfo.Connector;
                }
                else
                {
                    _managedServers.Add(serverStateInfo);
                    serverStateInfo.Connector = new BattleServerConnector();
                    serverStateInfo.Connector.Initialize(serverStateInfo.Ip, _serverList.port, _serverList.tryConnectionWaitingTime);
                    serverStateInfo.Connector.Closed += Connector_Closed;
                    serverStateInfo.Connector.Connected += Connector_Connected;
                    serverStateInfo.Connector.DataReceived += Connector_DataReceived;
                }
            }
        }

        private void ReloadServers()
        {
            if (!Monitor.TryEnter(_lockServerList))
                return;

            try
            {
                Config.GameServerReloadConfig();
                var reloadedServerList = _serverList.countryIps.SelectMany(countryip => countryip.Value.Select(ip => new ServerStateInfo
                {
                    Country = countryip.Key,
                    Ip = ip,
                    State = ServerStateType.Running,
                })).ToList();

                SetConnectors(reloadedServerList);

                var exceptServerList = _managedServers.Except(reloadedServerList, new ServerStateInfoComparer(false)).ToList();
                exceptServerList.ForEach(serverStateInfo => serverStateInfo.State = ServerStateType.UnManaged);

                _managedServers.Clear();
                _managedServers.AddRange(reloadedServerList);
                _managedServers.AddRange(exceptServerList);

                foreach (var server in _managedServers)
                {
                    Console.WriteLine(" S:" + server.Country + "ip:" + server.Ip + "STA:" +server.State);
                }
            }
            catch(Exception e)
            {
                AkaLogger.Log.Debug.Exception("ServerUpdator_ReloadServers:" + _jobName , e);

            }
            finally
            {
                Monitor.Exit(_lockServerList);
            }

            CheckSchedule();
        }

        private void Connector_DataReceived(object ip, BaseProtocol e)
        {
            var myIp = ip as string;
            var serverState = e as ProtoOnServerState;
            var serverList = new List<ServerStateInfo>(_managedServers);

            SetServerState(myIp, serverList, serverState.Sessions);
        }

        private void Connector_Connected(object ip, EventArgs e)
        {
            var myIp = ip as string;
            AkaLogger.Logger.Instance().Debug($"[ServerInfo] Connected:{myIp}");

            var data = AkaSerializer<ProtoServerState>.Serialize(new ProtoServerState());
            var existStateInfo = _managedServers.FirstOrDefault(currentStateInfo => currentStateInfo.Ip == myIp);
            if (existStateInfo != null)
            {
                existStateInfo.Connector.Send(MessageType.GetServerState, data);
            }
        }

        private void Connector_Closed(object ip, EventArgs e)
        {
            var myIp = ip as string;
            AkaLogger.Logger.Instance().Debug($"[ServerInfo] Closed:{myIp}");

            var data = AkaSerializer<ProtoServerState>.Serialize(new ProtoServerState());
            var serverList = new List<ServerStateInfo>(_managedServers);
            SetServerStateStopped(myIp, serverList);
            
        }

        private int MakeInt(ServerStateType stateType, int clientCount)
        {
            return (((byte)stateType) * ConstValue.SERVERINFO_ORDERING_MULTIPLE_NUM) + clientCount;
        }

        private void RequestServerState(List<ServerStateInfo> targetServerList)
        {
            var serverIpList = targetServerList.Distinct(new ServerStateInfoComparer(true));

            Parallel.ForEach(serverIpList, serverStateInfo =>
            {
                serverStateInfo.Connector.TryConnect();

                if (serverStateInfo.Connector.IsConnected())
                {
                    var data = AkaSerializer<ProtoServerState>.Serialize(new ProtoServerState());
                    serverStateInfo.Connector.Send(MessageType.GetServerState, data);
                }
                else
                {
                    SetServerStateStopped(serverStateInfo.Ip, targetServerList);
                }
            });
        }

        private void SetServerStateStopped(string ip, List<ServerStateInfo> targetServerList)
        {
            AkaLogger.Logger.Instance().Debug($"[ServerInfo] Stopped:{ip}");

            foreach (var stoppedServer in targetServerList.Where(serverInfo => serverInfo.Ip == ip))
            {
                ServerStatusInfoJob.SetServerStateInfo(_redis, _serverStatusRedisKey, stoppedServer.Country, ip, MakeInt(ServerStateType.Stopped, 0));
                if (stoppedServer.State == ServerStateType.UnManaged)
                    stoppedServer.State = ServerStateType.Expire;
            }
        }

        private void SetServerState(string ip, List<ServerStateInfo> targetServerList, int clientCount)
        {
            foreach (var server in targetServerList.Where(serverInfo => serverInfo.Ip == ip))
            {
                if (server.State != ServerStateType.Expire)
                    ServerStatusInfoJob.SetServerStateInfo(_redis, _serverStatusRedisKey, server.Country, ip, MakeInt(server.State, clientCount));

                if (server.State == ServerStateType.UnManaged && clientCount <= 1)
                    server.State = ServerStateType.Expire;

                AkaLogger.Logger.Instance().Debug($"[ServerInfo] Ip:{ip} Country:{server.Country} State:{server.State.ToString()} Count:{clientCount}");
            }
        }

        private void ExpireServerState()
        {
            if (!Monitor.TryEnter(_lockServerList))
                return;
            try
            {
                Parallel.ForEach(_managedServers, serverInfo =>
                {
                    if (serverInfo.State == ServerStateType.Expire)
                    {
                        ServerStatusInfoJob.RemoveServerStateInfo(_redis, _serverStatusRedisKey, serverInfo.Country, serverInfo.Ip);
                    }
                });

                _managedServers.RemoveAll(serverInfo => serverInfo.State == ServerStateType.Expire);
            }
            catch (Exception)
            {

            }
            finally
            {
                Monitor.Exit(_lockServerList);
            }
        }

        private  void CheckSchedule()
        {
            if (_checkTimer.IsTimerStatusStop)
                return;

            var targetServerList = new List<ServerStateInfo>(_managedServers);

            if (!Monitor.TryEnter(_lockSchedule))
                return;

            try
            {
                RequestServerState(targetServerList);
                ExpireServerState();
            }
            catch(Exception e)
            {
                AkaLogger.Log.Debug.Exception("ServerUpdator_CheckServerState:" + _jobName + " ServerList:"+ string.Join(",",targetServerList.Select( server=>server.Ip)), e);
            }
            finally
            {
                Monitor.Exit(_lockSchedule);
            }

        }
    }
}
