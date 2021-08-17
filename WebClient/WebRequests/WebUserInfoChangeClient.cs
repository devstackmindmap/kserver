using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebUserInfoChangeClient
    {
        public static ProtoResult Run(uint userId, UserAdditionalInfoType userInfoType, string userValue)
        {
            var datas = new ProtoUserInfoChange
            {
                MessageType = MessageType.UserAdditionalInfoChange,
                UserInfoType = userInfoType,
                UserId = userId,
                UserValue = userValue
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoUserInfoChange>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
