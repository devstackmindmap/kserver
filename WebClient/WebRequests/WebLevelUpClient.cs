using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebLevelUpClient
    {
        public static ProtoOnLevelUp Run(uint classId, PieceType pieceType, uint userId)
        {
            var datas = new ProtoLevelUp
            {
                MessageType = MessageType.LevelUp,
                ClassId = classId,
                PieceType = AkaEnum.PieceType.Unit,
                UserId = userId
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + WebUrlType.LevelUp.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoLevelUp>.Serialize(datas));

            return  AkaSerializer.AkaSerializer<ProtoOnLevelUp>.Deserialize(responseBytes);
        }
    }
}
