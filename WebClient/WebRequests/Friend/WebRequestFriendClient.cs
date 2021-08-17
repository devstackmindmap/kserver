using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebRequestFriendClient
    {
        public static ProtoResult Run(uint userId, uint friendId)
        {
            var datas = new ProtoRequestFriend
            {
                MessageType = MessageType.RequestFriendById,
                UserId = userId,
                FriendId = friendId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoRequestFriend>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
