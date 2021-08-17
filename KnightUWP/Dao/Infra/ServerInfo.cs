using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Dao
{
    public sealed class ServerInfos
    {
        public Dictionary<string,  Dictionary<string, ServerInfo>> Servers;
        public Ports Ports;
        public string DownloadUrl;

    }

    public sealed class ServerInfo
    {
        public string WebServerIp;
        public string MatchingServerIp;
        public string PubSubServerIp;
        public string ServiceStatus;
        public string Message;
        public int Version;
        public string TableData;
    }

    public sealed class Ports
    {
        public int WebServerPort;
        public int PubSubServerPort;
    }

}
