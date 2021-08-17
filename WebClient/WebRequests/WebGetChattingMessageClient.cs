using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetChattingMessageClient
    {
        public static ProtoOnGetChattingMessage Run()
        {
            var datas = new ProtoGetChattingMessage
            {
                MessageType = MessageType.GetChattingMessage,
                ChattingKey = "10"
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoGetChattingMessage>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnGetChattingMessage>.Deserialize(responseBytes);
        }
    }
}
