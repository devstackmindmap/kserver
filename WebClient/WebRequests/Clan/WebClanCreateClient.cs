using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebClanCreateClient
    {
        public static ProtoResult Run()
        {
            var datas = new ProtoClanCreate
            {
                MessageType = MessageType.ClanCreate,
                UserId = 24,
                ClanName = "리리리리리",
                ClanPublicType = AkaEnum.ClanPublicType.Private,
                ClanSymbolId = 3,
                JoinConditionRankPoint = 10,
                CountryCode = "KR",
                ClanExplain = "fdsafasd"
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoClanCreate>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
