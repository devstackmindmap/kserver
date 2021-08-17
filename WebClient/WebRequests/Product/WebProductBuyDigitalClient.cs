using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebProductBuyDigitalClient
    {
        public static ProtoOnBuyProductDigital Run()
        {
            var datas = new ProtoBuyProductDigital
            {
                MessageType = MessageType.BuyProductDigital,
                ProductId = 13,
                ProductTableType = ProductTableType.FixDigital,
                UserId = 1,
                ItemValue = 0
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoBuyProductDigital>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnBuyProductDigital>.Deserialize(responseBytes);
        }
    }
}
