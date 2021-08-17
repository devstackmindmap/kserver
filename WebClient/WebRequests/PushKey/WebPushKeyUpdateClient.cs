using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebPushKeyUpdateClient
    {
        public static ProtoResult Run(uint userId, string value)
        {
            var datas = new ProtoUserIdStringValue
            {
                MessageType = MessageType.UpdatePushKey,
                UserId = userId,
                Value = value
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdStringValue>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
