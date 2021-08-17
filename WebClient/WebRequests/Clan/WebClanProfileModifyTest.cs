using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebClanProfileModifyTest
    {
        public static ProtoResult Run(uint userId, uint clanId, ClanPublicType clanPublicType, 
            uint clanSymbolId, string countryCode, int joinConditionRankPoint)
        {
            var datas = new ProtoClanProfileModify
            {
                MessageType = MessageType.ClanProfileModify,
                UserId = userId,
                ClanExplain = "FDSAFDSAFSDAF",
                ClanId = clanId,
                ClanPublicType = clanPublicType,
                ClanSymbolId = clanSymbolId,
                CountryCode = countryCode,
                JoinConditionRankPoint = joinConditionRankPoint
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoClanProfileModify>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
