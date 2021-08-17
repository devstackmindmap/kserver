using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebChallengeRewardResetClient
    {
        public static ProtoResult Run()
        {
            var datas = new ProtoChallengeParam
            {
                MessageType = MessageType.ChallengeRewardReset,
                UserId = 7,
                Season = 1,
                Day = 1,
                DifficultLevel = 1
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoChallengeParam>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
