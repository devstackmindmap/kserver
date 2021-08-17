using CommonProtocol;
using System;

namespace PubSubServer
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.PubSubKeepAlive:
                    return null;
                case MessageType.PubSubLogin:
                    return new Login();
                case MessageType.PubSubMatchmaking:
                case MessageType.PubSubBattle:
                case MessageType.PubSubOnline:
                case MessageType.PubSubLogout:
                    return new StatusMessage();
                case MessageType.PubSubFriendAsked:
                case MessageType.PubSubFriendSigned:
                case MessageType.PubSubFriendRemoved:
                case MessageType.PubSubFriendlyBattleInvite: 
                case MessageType.PubSubFriendlyBattleAccept: 
                case MessageType.PubSubFriendlyBattleDecline: 
                case MessageType.PubSubFriendlyBattleCancel: 
                case MessageType.PubSubFriendlyBattleReady: 
                case MessageType.PubSubFriendlyBattleReadyCancel:
                    return new FriendMessage();
                case MessageType.PubSubUpdateQuest:
                    return new Web2OneMessage();
                case MessageType.PubSubClanJoin:
                case MessageType.PubSubClanOut:
                case MessageType.PubSubClanBanish:
                case MessageType.PubSubClanModifyMemberGradeUp:
                case MessageType.PubSubClanModifyMemberGradeDown:
                case MessageType.PubSubClanProfileModify:
                case MessageType.PubSubClanChatting:
                    return new ClanMessage();
                case MessageType.GetServerState:
                    return new ServerState();

                default:
                    throw new Exception("Invalid MessageType");
            }
        }
    }
}
