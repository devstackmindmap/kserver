using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetClanRecommendClient
    {
        public static ProtoClanRecommend Run(uint userId)
        {
            var datas = new ProtoUserId
            {
                MessageType = MessageType.GetClanRecommend,
                UserId = userId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoClanRecommend>.Deserialize(responseBytes);
        }
    }
}
