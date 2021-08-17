using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebGetRankingBoardUnitClient
    {
        public static ProtoOnRankingBoard Run(uint userId, uint unitId)
        {
            var datas = new ProtoRankingBoardUnit
            {
                MessageType = MessageType.GetRankingBoardUnit,
                UserId = userId,
                UnitId = unitId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoRankingBoardUnit>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnRankingBoard>.Deserialize(responseBytes);
        }
    }
}
