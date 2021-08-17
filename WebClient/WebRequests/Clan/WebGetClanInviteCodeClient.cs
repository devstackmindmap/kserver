using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetClanInviteCodeClient
    {
        public static ProtoInviteCode Run(uint userId)
        {
            var datas = new ProtoUserId
            {
                MessageType = MessageType.GetClanInviteCode,
                UserId = userId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoInviteCode>.Deserialize(responseBytes);
        }
    }
}
