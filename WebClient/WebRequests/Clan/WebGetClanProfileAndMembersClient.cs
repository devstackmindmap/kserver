using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetClanProfileAndMembersClient
    {
        public static ProtoClanProfileAndMembers Run(uint userId, uint targetId)
        {
            var datas = new ProtoUserIdTargetId
            {
                MessageType = MessageType.GetClanProfileAndMembers,
                UserId = userId,
                TargetId = targetId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdTargetId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoClanProfileAndMembers>.Deserialize(responseBytes);
        }
    }
}
