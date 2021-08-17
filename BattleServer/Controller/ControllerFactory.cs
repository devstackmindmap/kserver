using BattleServer.Controller.Controllers;
using CommonProtocol;
using System;

namespace BattleServer
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType battleMessageType)
        {
            switch (battleMessageType)
            {
                case MessageType.EnterLeagueRoom:
                    return new EnterLeagueRoom();
                case MessageType.EnterPveRoom:
                    return new EnterPvERoom();
                case MessageType.EnterChallengeRoom:
                    return new EnterChallengeRoom();
                case MessageType.EnterEventChallengeRoom:
                    return new EnterEventChallengeRoom();
                case MessageType.CardUse:
                    return new CardUse();
                case MessageType.ReEnterRoom:
                    return new ReEnterRoom();
                case MessageType.SyncTime:
                    return new SyncTime();
                case MessageType.Retreat:
                    return new Retreat();
                case MessageType.BattleReady:
                    return new BattleReady();
                case MessageType.EmoticonUse:
                    return new EmoticonUse();
                case MessageType.GetServerState:
                    return new ServerState();
                default:
                    throw new Exception("Invalid BattleMessageType");
            }
        }
    }
}
