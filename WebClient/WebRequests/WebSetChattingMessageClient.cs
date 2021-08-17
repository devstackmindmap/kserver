using System;
using System.Net;
using CommonProtocol;

namespace WebClient
{
    class WebSetChattingMessageClient
    {
        public static ProtoResult Run()
        {
            var datas = new ProtoSetChattingMessage
            {
                MessageType = MessageType.SetChattingMessage,
                UserNickname = "Dylan",
                ChattingKey = "10",
                ChattingMessage = "hi",
                ChattingType = AkaEnum.ChattingType.ClanBanish,
                TargetNickname = "Nick"
            };

            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes 
                = webClient.UploadData(new Uri("http://localhost:40654") + datas.MessageType.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoSetChattingMessage>.Serialize(datas));

            return AkaSerializer.AkaSerializer<ProtoResult>.Deserialize(responseBytes);
        }
    }
}
