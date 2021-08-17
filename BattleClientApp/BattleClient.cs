using AkaSerializer;
using CommonProtocol;
using Newtonsoft.Json;
using SuperSocket.ClientEngine;
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace BattleClientApp
{
    public partial class BattleClient : Form
    {
        AsyncTcpSession asyncBattleTcpSession = new AsyncTcpSession();
        AsyncTcpSession asyncMatchingTcpSession = new AsyncTcpSession();

        public BattleClient()
        {
            InitializeComponent();
        }

        public object WebUrlType { get; private set; }

        private void buttonMakeRoom_Click(object sender, EventArgs e)
        {
            var sr = new StreamReader(@"E:\Projects\KnightRun\kserver\Client2ServerInfo.json");
            string jsonString = sr.ReadToEnd();
            var ServerInfo = JsonConvert.DeserializeObject<ServerInfos>(jsonString).Servers["Local"];
  
            var getBattleServer = new ProtoGetBattleServer();
            var webClient = new System.Net.WebClient();
            webClient.Headers[HttpRequestHeader.ContentType] = "application/octet-stream";
            var responseBytes
                = webClient.UploadData(new Uri("http://" + ServerInfo.WebServerIp + ":" + ServerInfo.WebServerPort)
                + MessageType.GetBattleServer.ToString(), "POST"
                , AkaSerializer.AkaSerializer<ProtoGetBattleServer>.Serialize(getBattleServer));

            var battleServerInfo = AkaSerializer.AkaSerializer<ProtoOnGetBattleServer>.Deserialize(responseBytes);


            if (!asyncBattleTcpSession.IsConnected)
            {
                asyncBattleTcpSession.Connected += AsyncBattleTcpSession_Connected; ;
                asyncBattleTcpSession.DataReceived += AsyncBattleTcpSession_DataReceived;
                asyncBattleTcpSession.Connect(new IPEndPoint(IPAddress.Parse(battleServerInfo.BattleServerIp), battleServerInfo.BattleServerPort));
            }
        }

        private void AsyncBattleTcpSession_DataReceived(object sender, DataEventArgs e)
        {
            int offset = 0;
            var sizeHeader = sizeof(MessageType);
            while (offset < e.Length)
            {
                var header = e.Data.CloneRange(offset, sizeHeader);
                var body = e.Data.CloneRange(offset + sizeHeader, e.Length - sizeHeader);

                offset += e.Length;

                var msgType = (MessageType)BitConverter.ToInt32(header, 0);
                var proto = ProtocolFactory.DeserializeProtocol(msgType, body);

                var controller = ControllerFactory.CreateController(msgType);
                controller.DoPipeline(proto);
            }
        }

        private void AsyncBattleTcpSession_Connected(object sender, EventArgs e)
        {

        }

        private void AsyncMatchingTcpSession_Connected(object sender, EventArgs e)
        {

        }

        private void buttonTryMatching_Click(object sender, EventArgs e)
        {
            var sr = new StreamReader(@"E:\Projects\KnightRun\kserver\Client2ServerInfo.json");
            string jsonString = sr.ReadToEnd();
            var ServerInfo = JsonConvert.DeserializeObject<ServerInfos>(jsonString).Servers["Local"];

            if (!asyncMatchingTcpSession.IsConnected)
            {
                asyncMatchingTcpSession.Connected += AsyncMatchingTcpSession_Connected; ;
                asyncMatchingTcpSession.DataReceived += AsyncBattleTcpSession_DataReceived;
                asyncMatchingTcpSession.Connect(new IPEndPoint(IPAddress.Parse(ServerInfo.MatchingServerIp), ServerInfo.MatchingServerPort));
            }

            var header = BitConverter.GetBytes((int)MessageType.TryMatching);
            var body = AkaSerializer<ProtoTryMatching>.Serialize(new ProtoTryMatching
            {
                AreaNum = 1,
                UserId = 4,
                DeckNum = 0,
                DeckModeType = AkaEnum.ModeType.PVP,
                MessageType = MessageType.TryMatching
            });


            using (var stream = new MemoryStream(header.Length + body.Length))
            {
                stream.Write(header, 0, header.Length);
                stream.Write(body, 0, body.Length);
                stream.Seek(0, SeekOrigin.Begin);
                asyncMatchingTcpSession.Send(stream.GetBuffer(), 0, (int)stream.Length);
            }
        }
    }
}
