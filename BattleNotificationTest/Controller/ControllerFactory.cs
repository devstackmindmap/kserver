using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattleNotificationTest
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.MatchingSuccess:
                    return new MatchingSuccess();
                case MessageType.MatchingFail:
                    return new MatchingFail();
                case MessageType.BeforeBattleStart:
                    return new BeforeBattleStart();
                case MessageType.BattleStart:
                    return new Test();
                case MessageType.MatchingCancelSuccess:
                case MessageType.MatchingCancelFail:
                    return new MatchingCancel();
                case MessageType.StartBoosterTime:
                    return new Test();
                case MessageType.StartExtensionTime:
                case MessageType.Skill:
                case MessageType.AttackUnit:
                case MessageType.ValidateCard:
                    return new Test();
                case MessageType.ReEnterRoomSuccess:
                    return new ReEnterRoomSuccess();
                case MessageType.ReEnterRoomFail:
                    return new ReEnterRoomFail();
                case MessageType.GetBattleResult:
                    return new BattleResult();

                default:
                    throw new Exception("Invalid BattleMessageType");
            }
        }
    }
}
