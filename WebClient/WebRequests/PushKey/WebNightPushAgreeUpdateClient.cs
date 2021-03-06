using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebNightPushAgreeUpdateClient
    {
        public static ProtoResult Run(uint userId, byte agree)
        {
            var datas = new ProtoUserIdByteValue
            {
                MessageType = MessageType.UpdateNightPushAgree,
                UserId = userId,
                Value = agree
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdByteValue>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
