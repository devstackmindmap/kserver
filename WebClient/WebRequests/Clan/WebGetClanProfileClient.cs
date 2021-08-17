using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetClanProfileClient
    {
        public static ProtoClanProfile Run(uint userId, uint targetId)
        {
            var datas = new ProtoUserIdTargetId
            {
                MessageType = MessageType.GetClanProfile,
                UserId = userId,
                TargetId = targetId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdTargetId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoClanProfile>.Deserialize(responseBytes);
        }
    }
}
