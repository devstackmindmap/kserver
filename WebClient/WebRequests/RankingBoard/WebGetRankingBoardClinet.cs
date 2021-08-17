using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebGetRankingBoardClinet
    {
        public static ProtoOnRankingBoard Run(uint userId)
        {
            var datas = new ProtoRankingBoard
            {
                MessageType = MessageType.GetRankingBoard,
                UserId = userId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoRankingBoard>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnRankingBoard>.Deserialize(responseBytes);
        }
    }
}
