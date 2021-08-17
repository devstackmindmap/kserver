using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetEventChallengeFirstClearUserClient
    {
        public static ProtoOnGetDeckWithNickname Run()
        {
            var datas = new ProtoEventChallengeParam
            {
                MessageType = MessageType.GetEventChallengeFirstClearUser,
                UserId = 7,
                ChallengeEventId = 1, 
                DifficultLevel = 1
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoEventChallengeParam>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnGetDeckWithNickname>.Deserialize(responseBytes);
        }
    }
}
