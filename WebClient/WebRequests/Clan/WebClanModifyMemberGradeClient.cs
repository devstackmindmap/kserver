using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebClanModifyMemberGradeClient
    {
        public static ProtoResult Run(uint userId, uint targetId, ClanMemberGrade clanMemberGrade)
        {
            var datas = new ProtoModifyMemberGrade
            {
                MessageType = MessageType.ClanModifyMemberGrade,
                UserId = userId,
                TargetId = targetId,
                ClanMemberGrade = clanMemberGrade
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoModifyMemberGrade>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
