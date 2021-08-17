using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebGetFriendInfoClient
    {
        public static ProtoFriendInfo Run(uint userId)
        {
            var datas = new ProtoUserId
            {
                MessageType = MessageType.GetFriendInfo,
                UserId = userId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoFriendInfo>.Deserialize(responseBytes);
        }
    }
}
