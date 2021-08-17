using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebMailReadClient
    {
        public static ProtoMailActionResult Run(uint userId)
        {
            var datas = new ProtoMailRead
            {
                MessageType = MessageType.MailRead,
                UserId = userId,
                MailType = AkaEnum.MailType.Public,
                MailId = 77284
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoMailRead>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoMailActionResult>.Deserialize(responseBytes);
        }
    }
}
