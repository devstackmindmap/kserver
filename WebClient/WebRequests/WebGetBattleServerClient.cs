using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebGetBattleServerClient
    {
        public static ProtoOnGetBattleServer Run()
        {
            var datas = new ProtoGetBattleServer
            {
                MessageType = MessageType.GetBattleServer,
                GroupCode = 1
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoGetBattleServer>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnGetBattleServer>.Deserialize(responseBytes);
        }
    }
}
