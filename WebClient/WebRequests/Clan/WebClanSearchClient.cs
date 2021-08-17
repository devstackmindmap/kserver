using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebClanSearchClient
    {
        public static ProtoClanRecommend Run(string stringValue)
        {
            var datas = new ProtoString
            {
                MessageType = MessageType.ClanSearch,
                StringValue = stringValue
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoString>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoClanRecommend>.Deserialize(responseBytes);
        }
    }
}
