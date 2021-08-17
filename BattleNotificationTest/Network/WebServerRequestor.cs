using AkaSerializer;
using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class WebServerRequestor
    {
        public WebServerRequestor()
        {

        }

        private static WebServerRequestor _instance = null;
        public static WebServerRequestor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new WebServerRequestor();
                }
                return _instance;
            }
        }

        public byte[] Request(MessageType messageType, byte[] reqData)
        {
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            return webClient.UploadData(new Uri(C2SInfo.Instance.WebUri)
                + messageType.ToString(), "POST"
                , reqData);
        }
    }
}
