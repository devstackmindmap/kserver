using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebAddFriendByInviteCodeClient
    {
        public static ProtoFriendInfo Run(uint userId, string friendCode)
        {
            var datas = new ProtoAddInvite
            {
                MessageType = MessageType.AddFriendByCode,
                UserId = userId,
                InviteCode = friendCode
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoAddInvite>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoFriendInfo>.Deserialize(responseBytes);
        }
    }
}
