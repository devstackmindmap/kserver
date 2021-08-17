using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquareObjectSimulator.Services
{
    using AkaConfig;
    using AkaData;
    using AkaEnum;
    using AkaUtility;
    using DAO;
    using System.Reflection;

    public class UserInfo
    {
        public Dictionary<string, UserDAO> Users { get; private set; }

        public static UserInfo Instance { get; private set; }

        public List<RunMode> RunModes => Enum.GetValues(typeof(RunMode)).Cast<RunMode>().ToList();


        private RunMode CurrentRunMode = RunMode.JoyOne;


        static UserInfo()
        {
            Instance = new UserInfo();
        }

        public UserInfo()
        {
             Users = new Dictionary<string, UserDAO>();
        }

        public void LoadDatas(string userid, string runmodeString)
        {
            var runmode = RunModes.FirstOrDefault(runmode => runmode.ToString().ToLower() == runmodeString.ToLower());

            var user = Get(userid);
            if (user != null)
            {
                CurrentRunMode = runmode;
                user.SelectedRunMode = CurrentRunMode;
                foreach(var alluser in Users.Values)
                {
                    alluser.SelectedRunMode = CurrentRunMode;
                }

                Config.CommonServerInitConfig(Server.GameServer, user.SelectedRunMode.ToString());
                Config.GameServerReloadConfig();
                DataGetter.StoreDataMap.Clear();

                var loader = new FileLoader(FileType.Table, Config.RunMode, 0);
                var taskResult = loader.GetFileLists();

                taskResult.Wait();

                DataSetter dataSetter = new DataSetter();
                dataSetter.DataSet(taskResult.Result);

                Config.CommonServerConfig = null;
            }
        }

        public void Init()
        {
            //TODO load init

        }


        public bool Add(string id)
        {
            var created = Users.TryAdd(id, new UserDAO { Id = id , SelectedRunMode = CurrentRunMode, SO = new List<int>()});
            if (created)
            {
            }
            return created;
        }

        public bool Delete(string id)
        {
            if (Users.TryGetValue(id, out var user))
                SquareObject.Instance.Delete(user.SO);
            return Users.Remove(id);
        }

        public UserDAO Get(string id )
        {
            return Users.TryGetValue(id, out var obj) ? obj : null;
        }

        public List<UserDAO> ToList()
        {
            return Users.Select(user => user.Value).ToList();
        }
    }
}
