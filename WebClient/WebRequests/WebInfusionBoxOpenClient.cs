using System;
using System.Net;
using AkaEnum;
using CommonProtocol;

namespace WebClient
{
    class WebInfusionBoxOpenClient
    {
        public static ProtoOnInfusionBoxOpen Run(InfusionBoxType infusionBoxType, uint userId)
        {
            var datas = new ProtoInfusionBoxOpen
            {
                MessageType = MessageType.InfusionBoxOpen,
                Type = infusionBoxType, //InfusionBoxType.LeagueBox,
                UserId = userId 
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoInfusionBoxOpen>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoOnInfusionBoxOpen>.Deserialize(responseBytes);
        }
    }
}
