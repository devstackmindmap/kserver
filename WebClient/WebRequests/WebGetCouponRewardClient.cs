using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetCouponRewardClient

    {
        public static ProtoGetCouponReward Run()
        {
            var datas = new ProtoUserIdStringValue
            {
                MessageType = MessageType.GetCouponReward,
                UserId = 1,
                Value = "Z6E7HF3PDMJUXLE9"
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserIdStringValue>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoGetCouponReward>.Deserialize(responseBytes);
        }
    }
}
