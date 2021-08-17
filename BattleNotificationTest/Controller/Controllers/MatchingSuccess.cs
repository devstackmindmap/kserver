using AkaSerializer;
using CommonProtocol;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public class MatchingSuccess : BaseController
    {
        public override async Task DoPipeline(BaseProtocol requestInfo, BattleNotificationForm form)
        {
            if (form != null)
                form.Notice("MatchingSuccess");
            var req = requestInfo as ProtoMatchingSuccess;

            MatchingServerConnector.Instance.Close();
            BattleServerConnector.Instance.Connect(req.BattleServerIp, C2SInfo.Instance.ServerInfo.BattleServerPort);

            while (!BattleServerConnector.Instance.IsConnected())
            {
                System.Threading.Thread.Sleep(100);
            }

            BattleServerConnector.Instance.Send(MessageType.EnterRoom, AkaSerializer<ProtoEnterRoom>.Serialize(new ProtoEnterRoom
            {
                MessageType = MessageType.EnterRoom,
                UserId = form.GetUserId(),
                RoomId = req.RoomId,
                BattleType=AkaEnum.Battle.BattleType.LeagueBattle,

                DeckNum = form.GetDeckNum(),
                BattleServerIp = req.BattleServerIp
            }));

            //PlayerInfo.Instance.RoomId = req.RoomId;
        }
    }
}
