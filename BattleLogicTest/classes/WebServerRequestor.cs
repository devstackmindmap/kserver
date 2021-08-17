using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AkaConfig;
using AkaData;
using AkaDB;
using AkaEnum;
using AkaEnum.Battle;
using AkaRedis;
using AkaSerializer;
using CommonProtocol;
using NUnit.Framework;



namespace BattleLogicTest
{
    public class WebServerRequestor
    {
        public WebServerRequestor()
        {

        }

        public byte[] Request(MessageType messageType, byte[] reqData, string webUrl)
        {
            var webClient = new WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            return webClient.UploadData(new Uri(webUrl)
                + messageType.ToString(), "POST"
                , reqData);
        }
    }
}
