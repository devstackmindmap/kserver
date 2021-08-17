using Microsoft.AspNetCore.Http;
using System;
using WebServer.Controller.Battle;
using WebServer.Controller.Box;
using WebServer.Controller.Challenge;
using WebServer.Controller.Chatting;
using WebServer.Controller.Clan;
using WebServer.Controller.Coupon;
using WebServer.Controller.Deck;
using WebServer.Controller.Emoticon;
using WebServer.Controller.Event;
using WebServer.Controller.Friend;
using WebServer.Controller.Item;
using WebServer.Controller.LevelUp;
using WebServer.Controller.Mail;
using WebServer.Controller.Matching;
using WebServer.Controller.Quest;
using WebServer.Controller.Rank;
using WebServer.Controller.Rewards;
using WebServer.Controller.SquareObject;
using WebServer.Controller.Stage;
using WebServer.Controller.Store;
using WebServer.Controller.Test;
using WebServer.Controller.User;
using MessageType = CommonProtocol.MessageType;

namespace WebServer
{
    public static class ControllerFactory
    {
        public static BaseController CreateController(MessageType messageType, HttpContext context)
        {
            switch(messageType)
            {
                case MessageType.ServerStatus:
                    return new WebServerStatus(context);
                case MessageType.Login:
                    return new WebLogin(context);
                case MessageType.AccountJoin:
                    return new WebAccountJoin(context);
                case MessageType.LevelUp:
                    return new WebLevelUp();
                case MessageType.SetDeck:
                    return new WebSetDeck();
                case MessageType.GetDeck:
                    return new WebGetDeck();
                case MessageType.GetProducts:
                    return new WebGetProducts();
                case MessageType.BuyProductDigital:
                    return new WebProductBuyDigital();
                case MessageType.BuyProductReal:
                    return new WebProductBuyReal();
                case MessageType.InfusionBoxOpen:
                    return new WebInfusionBoxOpen();
                case MessageType.GetBattleServer:
                    return new WebGetBattleServer();
                case MessageType.GetDeckWithDeckNum:
                    return new WebGetDeckWithDeckNum();
                case MessageType.GetBattleResult:
                    return new WebGetBattleResultRoguelike();
                case MessageType.GetBattleResultKnightLeague:
                    return new WebGetBattleResultRank();
                case MessageType.GetBattleResultVirtualLeague:
                    return new WebGetBattleResultVirtualLeague();
                case MessageType.GetBattleResultChallenge:
                    return new WebGetBattleResultChallenge();
                case MessageType.GetBattleResultEventChallenge:
                    return new WebGetBattleResultEventChallenge();
                case MessageType.SkinPutOn:
                    return new WebSkinPutOn();
                case MessageType.SaveBattleRecordInfo:
                    return new WebSaveBattleRecord();
                case MessageType.GetBattleRecord:
                    return new WebGetBattleRecord();
                case MessageType.GetBattleRecordList:
                    return new WebGetBattleRecordList();
                case MessageType.GetVirtualRankPoint:
                    return new WebGetVirtualRankPoint();
                case MessageType.GetRankPoint:
                    return new WebGetTeamRankPoint();
                case MessageType.GetStageLevelRoomInfo:
                    return new WebStageLevelRoomInfo();
                case MessageType.SetStageLevelRoomInfo:
                    return new WebSetStageLevelRoomInfo();
                case MessageType.SetSaveDeck:
                    return new WebSetSaveDeck();
                case MessageType.GetRankingBoard:
                    return new WebGetRankingBoard();
                case MessageType.GetRankingBoardClan:
                    return new WebGetRankingBoardClan();
                case MessageType.GetRankingBoardUnit:
                    return new WebGetRankingBoardUnit();
                case MessageType.SetEmoticons:
                    return new WebSetEmoticon();
                case MessageType.SetQuest:
                    return new WebSetQuest();
                case MessageType.GetQuestList:
                    return new WebGetQuest();
                case MessageType.GetFriendCode:
                    return new WebGetFriendCode();
                case MessageType.AddFriendByCode:
                    return new WebAddFriendByCode();
                case MessageType.AddFriendByRequested:
                    return new WebAddFriendByRequested();
                case MessageType.RejectFriendByRequested:
                    return new WebRejectFriendByRequested();
                case MessageType.RequestFriendById:
                    return new WebRequestFriendById();
                case MessageType.RequestFriendByNickname:
                    return new WebRequestFriendByNickname();
                case MessageType.GetReward:
                    return new WebGetReward();
                case MessageType.GetFriendInfo:
                    return new WebGetFriendInfo();
                case MessageType.RemoveFriend:
                    return new WebRemoveFriend();
                case MessageType.GetSquareObjectState:
                    return new WebGetSquareObject();
                case MessageType.SquareObjectStart:
                    return new WebSquareObjectStart();
                case MessageType.SquareObjectStop:
                    return new WebSquareObjectStop(); 
                case MessageType.SquareObjectPowerInjection:
                    return new WebSquareObjectPowerInject();
                case MessageType.GetUserProfile:
                    return new WebGetUserProfile();
                case MessageType.GetUnitProfile:
                    return new WebGetUnitProfile();
                case MessageType.GetCardProfile:
                    return new WebGetCardProfile();
                case MessageType.ChangeProfileIcon:
                    return new WebChangeProfileIcon();
                case MessageType.Test:
                    return new WebTestCallLockTest();
                case MessageType.ClanCreate:
                    return new WebClanCreate();
                case MessageType.GetClanRecommend:
                    return new WebGetClanRecommend();
                case MessageType.GetClanInviteCode:
                    return new WebGetClanInviteCode();
                case MessageType.ClanJoin:
                    return new WebClanJoin();
                case MessageType.ClanJoinByCode:
                    return new WebClanJoinByCode();
                case MessageType.GetClanProfile:
                    return new WebGetClanProfile();
                case MessageType.GetClanProfileAndMembers:
                    return new WebGetClanProfileAndMembers();
                case MessageType.ClanOut:
                    return new WebClanOut();
                case MessageType.ClanBanish:
                    return new WebClanBanish();
                case MessageType.ClanSearch:
                    return new WebClanSearch();
                case MessageType.ClanModifyMemberGrade:
                    return new WebClanModifyMemberGrade();
                case MessageType.ClanProfileModify:
                    return new WebClanProfileModify();
                case MessageType.UserAdditionalInfoChange:
                    return new WebUserAdditionalInfoChange();
                case MessageType.GetAdditionalUserInfo:
                    return new WebGetUserAdditionalInfo();
                case MessageType.UpdatePushKey:
                    return new WebPushKeyUpdate();
                case MessageType.UpdatePushAgree:
                    return new WebPushAgreeUpdate();
                case MessageType.UpdateNightPushAgree:
                    return new WebNightPushAgreeUpdate();
                case MessageType.UpdateTermsAgree:
                    return new WebTermsAgreeUpdate();
                case MessageType.GetSeasonReward:
                    return new WebGetSeasonReward();
                case MessageType.SetChattingMessage:
                    return new WebSetChattingMessage();
                case MessageType.GetChattingMessage:
                    return new WebGetChattingMessage();
                case MessageType.GetCouponReward:
                    return new WebGetCouponReward();
                case MessageType.MailRead:
                    return new WebMailRead();
                case MessageType.MailReadAll:
                    return new WebMailReadAll();
                case MessageType.MailDeleteAll:
                    return new WebMailDeleteAll();
                case MessageType.MailUpdatePublic:
                    return new WebMailUpdatePublic();
                case MessageType.MailUpdatePrivate:
                    return new WebMailUpdatePrivate();
                case MessageType.MailUpdateSystem:
                    return new WebMailUpdateSystem();
                case MessageType.BuySeasonPass:
                    return new WebProductBuySeasonPass();
                case MessageType.SkipQuest:
                    return new WebSkipQuest();
                case MessageType.NewQuest:
                    return new WebNewQuest();
                case MessageType.StartChallenge:
                    return new WebStartChallenge();
                case MessageType.StartEventChallenge:
                    return new WebStartEventChallenge();
                case MessageType.GetChallengeStageList:
                    return new WebGetChallengeStageList();
                case MessageType.GetEventChallengeStageList:
                    return new WebGetEventChallengeStageList();
                case MessageType.GetChallengeFirstClearUser:
                    return new WebGetChallengeFirstClearUser();
                case MessageType.GetEventChallengeFirstClearUser:
                    return new WebGetEventChallengeFirstClearUser();
                case MessageType.BattleRoundClearChallenge:
                    return new WebChallengeRoundClear();
                case MessageType.BattleRoundClearEventChallenge:
                    return new WebEventChallengeRoundClear();
                case MessageType.SyncTime:
                    return new WebSyncTime();
                case MessageType.GetSquareObjectFriends:
                    return new WebGetSquareObjectFriends();
                case MessageType.SquareObjectPowerInjectionFriend:
                    return new WebSquareObjectPowerInjectFriend();
                case MessageType.SquareObjectReactivate:
                    return new WebSquareObjectReactivate();
                case MessageType.SquareObjectBuyEnergy:
                    return new WebSquareObjectBuyEnergy();
                case MessageType.EventAddFriendByCode:
                    return new WebEventAddFriendByCode();
                case MessageType.EventFriendGetReward:
                    return new WebEventFriendGetReward();
                case MessageType.EventFriendCheck:
                    return new WebEventFriendCheck();
                case MessageType.ChallengeRewardReset:
                    return new WebChallengeRewardReset();
                case MessageType.EventChallengeRewardReset:
                    return new WebEventChallengeRewardReset();
                case MessageType.ReloadServerList:
               //     return new WebReloadServerList();
                default:
                    throw new Exception("[ControllerFactory] Invalid Message Type : " + messageType);
            }
        }
    }
}
