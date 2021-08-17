using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetCardProfileClient
    {
        public static ProtoOnCardProfile Run()
        {
            var datas = new ProtoUserIdAndId
            {
                MessageType = MessageType.GetCardProfile,
                Id = 1001,
                UserId = 129
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdAndId>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnCardProfile>.Deserialize(responseBytes);
        }
    }
}
