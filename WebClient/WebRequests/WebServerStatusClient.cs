using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebServerStatusClient
    {
        public static ProtoResult Run()
        {
            var datas = new ProtoEmpty
            {
                MessageType = MessageType.ServerStatus
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoEmpty>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
