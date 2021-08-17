using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BattleNotificationTest
{
    public class C2SInfo
    {
        public ServerInfo ServerInfo;
        public string WebUri;

        private C2SInfo(string rumMode)
        {
            var sr = new StreamReader(@"..\..\..\Client2ServerInfo.json");
            string jsonString = sr.ReadToEnd();
            ServerInfo = JsonConvert.DeserializeObject<ServerInfos>(jsonString).Servers[rumMode];
            WebUri = "http://" + ServerInfo.WebServerIp + ":" + ServerInfo.WebServerPort;
        }

        private static C2SInfo _instance = null;
        public static C2SInfo InstanceInit(string rumMode)
        {
            if (_instance == null)
            {
                _instance = new C2SInfo(rumMode);
            }
            return _instance;
        }

        public static C2SInfo Instance
        {
            get
            {
                return _instance;
            }
        }
    }
}
