using System;
using System.Collections.Generic;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebMailUpdatePublic
    {
        public static ProtoPublicMailInfo Run(uint userId)
        {
            var datas = new ProtoMailUpdatePublic
            {
                MessageType = MessageType.MailUpdatePublic,
                UserId = userId,
                LanguageType = "KR"
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoMailUpdatePublic>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoPublicMailInfo>.Deserialize(responseBytes);
        }
    }
}
