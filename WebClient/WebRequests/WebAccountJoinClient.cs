using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebAccountJoinClient
    {
        public static ProtoOnLogin Run()
        {
            var datas = new ProtoAccountJoin
            {
                MessageType = MessageType.AccountJoin,
                SocialAccount = "kdjkjfnm3",
                NickName = "kdjkjfnm3",
                PlatformType = PlatformType.Apple,
                LanguageType = "KR"
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoAccountJoin>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnLogin>.Deserialize(responseBytes);
        }
    }
}
