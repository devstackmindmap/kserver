using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebLoginClient
    {
        public static ProtoOnLogin Run(string nickName)
        {
            var datas = new ProtoLogin
            {
                MessageType = MessageType.Login,
                SocialAccount = "fdasf",
                LanguageType = "KR",
                PlatformType = PlatformType.Google
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoLogin>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnLogin>.Deserialize(responseBytes);
        }
    }
}
