using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetChallengeStageListClient
    {
        public static ProtoChallengeStageList Run()
        {
            var datas = new ProtoUserId
            {
                MessageType = MessageType.GetChallengeStageList,
                UserId = 7
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoChallengeStageList>.Deserialize(responseBytes);
        }
    }
}
