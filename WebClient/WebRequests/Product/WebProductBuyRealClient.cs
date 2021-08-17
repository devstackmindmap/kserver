using System;
using System.Collections.Generic;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebProductBuyRealClient
    {
        public static ProtoOnBuyProductReal Run()
        {
            var storeInfos = new List<ProtoStoreInfo>();
            storeInfos.Add(new ProtoStoreInfo
            {
                PurchaseToken = "fdsafasdf",
                StoreProductId = "",
                TransactionId = "",
                PlatformType = PlatformType.Google
            });
            

            var datas = new ProtoBuyProductReal
            {
                MessageType = MessageType.BuyProductReal,
                StoreInfos = storeInfos
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoBuyProductReal>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnBuyProductReal>.Deserialize(responseBytes);
        }
    }
}
