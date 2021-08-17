using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebSkinPutOnClient
    {
        public static ProtoResult Run()
        {
            var datas = new ProtoSkinPutOn
            {
                MessageType = MessageType.SkinPutOn,
                UserId = 39,
                SkinId = 0,
                UnitId = 1005
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoSkinPutOn>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
