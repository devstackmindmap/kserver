using System;
using System.Collections.Generic;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebMailDeleteAllClient
    {
        public static ProtoMailReadAllResult Run(uint userId)
        {
            var mailIds = new List<uint>();
            mailIds.Add(1); 
            mailIds.Add(2); 
            //mailIds.Add(3); 
            //mailIds.Add(4); 
            //mailIds.Add(5);
            //mailIds.Add(6);
            //mailIds.Add(7);
            //mailIds.Add(77280);
            //mailIds.Add(77281);
            //mailIds.Add(77282);
            //mailIds.Add(77283);
            var datas = new ProtoMailReadAll
            {
                MessageType = MessageType.MailDeleteAll,
                UserId = userId,
                MailType = AkaEnum.MailType.System,
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
