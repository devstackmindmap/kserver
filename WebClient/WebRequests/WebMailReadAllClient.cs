using System;
using System.Collections.Generic;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebMailReadAllClient
    {
        public static ProtoMailReadAllResult Run(uint userId)
        {
            var mailIds = new List<uint>();
            mailIds.Add(1); 
            mailIds.Add(2); 
            mailIds.Add(3); 
            mailIds.Add(4); 
            mailIds.Add(5);
            var datas = new ProtoMailReadAll
            {
                MessageType = MessageType.MailReadAll,
                UserId = userId,
                MailType = AkaEnum.MailType.Private,
                MailIds = mailIds
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoMailReadAll>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoMailReadAllResult>.Deserialize(responseBytes);
        }
    }
}
