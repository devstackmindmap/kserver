using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebGetProductsClient
    {
        public static ProtoOnGetProducts Run(uint userId, string currencyType)
        {
            var datas = new ProtoGetProducts
            {
                MessageType = MessageType.GetProducts,
                UserId = userId,
                LanguageType = "KR",
                PlatformType = PlatformType.Google
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoGetProducts>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnGetProducts>.Deserialize(responseBytes);
        }
    }
}
