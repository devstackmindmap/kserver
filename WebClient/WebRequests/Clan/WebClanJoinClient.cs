using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebClanJoinClient
    {
        public static ProtoClanJoinResult Run(uint userId, uint targetId)
        {
            var datas = new ProtoUserIdTargetId
            {
                MessageType = MessageType.ClanJoin,
                UserId = userId,
                TargetId = targetId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdTargetId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoClanJoinResult>.Deserialize(responseBytes);
        }
    }
}
