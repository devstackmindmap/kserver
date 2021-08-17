using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebClanJoinByCodeClient
    {
        public static ProtoClanJoinResult Run(uint userId, string inviteCode)
        {
            var datas = new ProtoAddInvite
            {
                MessageType = MessageType.ClanJoinByCode,
                UserId = userId,
                InviteCode = inviteCode
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoAddInvite>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoClanJoinResult>.Deserialize(responseBytes);
        }
    }
}
