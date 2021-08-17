using AkaSerializer;
using System;
using System.IO;

namespace CommonProtocol
{
    public static class Tcp2Client
    {
        public static BaseProtocol DeserializeProtocol(MessageType messageType, byte[] bytes)
        {
            switch (messageType)
            {
                case MessageType.EnterLeagueRoom:
                case MessageType.EnterRoom:
                    return AkaSerializer<ProtoEnterRoom>.Deserialize(bytes);
                case MessageType.EnterPveRoom:
                    return AkaSerializer<ProtoEnterPveRoom>.Deserialize(bytes);
                case MessageType.EnterChallengeRoom:
                    return AkaSerializer<ProtoEnterChallengeRoom>.Deserialize(bytes);
                case MessageType.EnterEventChallengeRoom:
                    return AkaSerializer<ProtoEnterEventChallengeRoom>.Deserialize(bytes);
                case MessageType.InfusionBoxOpen:
                    return AkaSerializer<ProtoInfusionBoxOpen>.Deserialize(bytes);
                case MessageType.Login:
                    return AkaSerializer<ProtoLogin>.Deserialize(bytes);
                case MessageType.LevelUp:
                    return AkaSerializer<ProtoLevelUp>.Deserialize(bytes);
                case MessageType.SetDeck:
                    return AkaSerializer<ProtoSetDeck>.Deserialize(bytes);
                case MessageType.GetDeck:
                    return AkaSerializer<ProtoGetDeck>.Deserialize(bytes);
                case MessageType.GetBattleServer:
                    return AkaSerializer<ProtoGetBattleServer>.Deserialize(bytes);
                case MessageType.TryMatching:
                    return AkaSerializer<ProtoTryMatching>.Deserialize(bytes);
                case MessageType.TryFvFMatching:
                    return AkaSerializer<ProtoTryFvFMatching>.Deserialize(bytes);
                case MessageType.MatchingSuccess:
                    return AkaSerializer<ProtoMatchingSuccess>.Deserialize(bytes);
                case MessageType.MatchingFail:
                    return AkaSerializer<ProtoMatchingFail>.Deserialize(bytes);
                case MessageType.BeforeBattleStart:
                    return AkaSerializer<ProtoBeforeBattleStart>.Deserialize(bytes);
                case MessageType.BattleStart:
                    return AkaSerializer<ProtoBattleStart>.Deserialize(bytes);
                case MessageType.BattleReady:
                    return AkaSerializer<ProtoBattleReady>.Deserialize(bytes);
                case MessageType.MatchingCancel:
                    return AkaSerializer<ProtoMatchingCancel>.Deserialize(bytes);
                case MessageType.MatchingCancelSuccess:
                case MessageType.MatchingCancelFail:
                case MessageType.ReEnterRoomFail:
                case MessageType.Skill:
                case MessageType.AttackUnit:
                case MessageType.StartExtensionTime:
                case MessageType.StartBoosterTime:
                    return AkaSerializer<ProtoEmpty>.Deserialize(bytes);
                case MessageType.CardUse:
                    return AkaSerializer<ProtoCardUse>.Deserialize(bytes);
                case MessageType.CardUseResult:
                    return AkaSerializer<ProtoCardUseResult>.Deserialize(bytes);
                case MessageType.ReEnterRoom:
                    return AkaSerializer<ProtoReEnterRoom>.Deserialize(bytes);
                case MessageType.ReEnterRoomSuccess:
                    return AkaSerializer<ProtoCurrentBattleStatus>.Deserialize(bytes);
                case MessageType.GetDeckWithDeckNum:
                    return AkaSerializer<ProtoGetDeckWithDeckNum>.Deserialize(bytes);
                case MessageType.GetBattleResult:
                    return AkaSerializer<ProtoOnBattleResultList>.Deserialize(bytes);
                case MessageType.GetBattleResultKnightLeague:
                    return AkaSerializer<ProtoOnBattleResultRankList>.Deserialize(bytes);
                case MessageType.GetBattleResultVirtualLeague:
                    return AkaSerializer<ProtoOnBattleResultRankData>.Deserialize(bytes);
                case MessageType.GetBattleResultChallenge:
                case MessageType.GetBattleResultEventChallenge:
                    return AkaSerializer<ProtoOnBattleResult>.Deserialize(bytes);
                case MessageType.ValidateCard:
                    return AkaSerializer<ProtoValidateCard>.Deserialize(bytes);
                case MessageType.SyncTime:
                    return AkaSerializer<ProtoSyncTime>.Deserialize(bytes);
                case MessageType.Retreat:
                    return AkaSerializer<ProtoRetreat>.Deserialize(bytes);
                case MessageType.SkinPutOn:
                    return AkaSerializer<ProtoSkinPutOn>.Deserialize(bytes);
                case MessageType.GetBattleRecord:
                    return AkaSerializer<ProtoOnGetBattleRecord>.Deserialize(bytes);
                case MessageType.GetBattleRecordList:
                    return AkaSerializer<ProtoGetBattleRecordList>.Deserialize(bytes);
                case MessageType.SaveBattleRecordInfo:
                    return AkaSerializer<ProtoBattleRecord>.Deserialize(bytes);
                case MessageType.GetVirtualRankPoint:
                case MessageType.GetRankPoint:
                    return AkaSerializer<ProtoRankPoint>.Deserialize(bytes);
                case MessageType.EmoticonUse:
                    return AkaSerializer<ProtoEmoticonUse>.Deserialize(bytes);
                case MessageType.PubSubLogin:
                    return AkaSerializer<PubSub.ProtoLogin>.Deserialize(bytes);
                case MessageType.PubSubOnLogin:
                case MessageType.PubSubOnOtherLogin:
                case MessageType.PubSubLogout:
                case MessageType.PubSubMatchmaking:
                case MessageType.PubSubBattle:
                case MessageType.PubSubOnline:
                case MessageType.PubSubOnFriendAsked:
                case MessageType.PubSubOnFriendSigned:
                case MessageType.PubSubOnFriendRemoved:
                case MessageType.PubSubOnFriendlyBattleInvite:
                case MessageType.PubSubOnFriendlyBattleAccept:
                case MessageType.PubSubOnFriendlyBattleDecline:
                case MessageType.PubSubOnFriendlyBattleCancel:
                case MessageType.PubSubOnFriendlyBattleReady:
                case MessageType.PubSubOnFriendlyBattleReadyCancel:
                case MessageType.PubSubOnClanJoin:
                case MessageType.PubSubOnClanOut:
                    return AkaSerializer<PubSub.ProtoOne2One>.Deserialize(bytes);
                case MessageType.PubSubFriendAsked:
                case MessageType.PubSubFriendSigned:
                case MessageType.PubSubFriendRemoved:
                case MessageType.PubSubFriendlyBattleInvite:
                case MessageType.PubSubFriendlyBattleAccept:
                case MessageType.PubSubFriendlyBattleDecline:
                case MessageType.PubSubFriendlyBattleCancel:
                case MessageType.PubSubFriendlyBattleReady:
                case MessageType.PubSubFriendlyBattleReadyCancel:
                    return AkaSerializer<PubSub.ProtoOne2N>.Deserialize(bytes);
                case MessageType.PubSubUpdateQuest:
                    return AkaSerializer<PubSub.ProtoWeb2OneUpdateQuest>.Deserialize(bytes);
                case MessageType.PubSubKeepAlive:
                    return AkaSerializer<PubSub.ProtoKeepAlive>.Deserialize(bytes);
                case MessageType.PubSubPublicNotice:
                    return AkaSerializer<CommonProtocol.PubSub.ProtoPubsubNotice>.Deserialize(bytes);
                case MessageType.PubSubClanProfileModify:
                    return AkaSerializer<CommonProtocol.PubSub.ProtoClanId>.Deserialize(bytes);
                case MessageType.PubSubClanJoin:
                    return AkaSerializer<CommonProtocol.PubSub.ProtoClanJoin>.Deserialize(bytes);
                case MessageType.PubSubClanOut:
                case MessageType.PubSubOnClanBanish:
                case MessageType.PubSubOnClanModifyMemberGradeUp:
                case MessageType.PubSubOnClanModifyMemberGradeDown:
                    return AkaSerializer<CommonProtocol.ProtoUserIdTargetId>.Deserialize(bytes);
                case MessageType.PubSubClanBanish:
                case MessageType.PubSubClanModifyMemberGradeUp:
                case MessageType.PubSubClanModifyMemberGradeDown:
                    return AkaSerializer<CommonProtocol.ProtoUserIdTargetIdClanId>.Deserialize(bytes);
                case MessageType.PubSubClanChatting:
                    return AkaSerializer<PubSub.ProtoClanChatting>.Deserialize(bytes);
                case MessageType.PubSubOnClanChatting:
                    return AkaSerializer<CommonProtocol.ProtoUserIdStringValue>.Deserialize(bytes);

                case MessageType.GetServerState:
                    return AkaSerializer<CommonProtocol.ProtoServerState>.Deserialize(bytes);
                case MessageType.OnGetServerState:
                    return AkaSerializer<CommonProtocol.ProtoOnServerState>.Deserialize(bytes);
                case MessageType.SkipQuest:
                    return AkaSerializer<CommonProtocol.ProtoUserId>.Deserialize(bytes);

                case MessageType.ReloadServerList:
                    return AkaSerializer<CommonProtocol.ProtoReloadServerList>.Deserialize(bytes);


                //For Test Client
                case MessageType.BattleEnd:
                    return AkaSerializer<CommonProtocol.ProtoBattleEnd>.Deserialize(bytes);
                case MessageType.BattleChallengeRoundResult:
                case MessageType.BattleEventChallengeRoundResult:
                    return AkaSerializer<CommonProtocol.ProtoEmpty>.Deserialize(bytes);
                case MessageType.EnqueuedAction:
                    return AkaSerializer<CommonProtocol.ProtoEnqueuedAction>.Deserialize(bytes);
                case MessageType.EnterRoomFail:
                    return AkaSerializer<CommonProtocol.ProtoResult>.Deserialize(bytes);
                case MessageType.UpdateUnitAttackTime:
                    return AkaSerializer<CommonProtocol.Battle.ProtoUpdateUnitAttackTime>.Deserialize(bytes);
                default:
                    throw new Exception("[ProtocolFactory] Invalid Message Type : " + messageType);
            }
        }
    }
}
