using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using CommonProtocol;
using CommonProtocol.Login;
using KnightUWP.Dao;
using Network;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Net
{
    static class API
    {
        private static int Port => ProfileManager.ServerInfo.Ports.WebServerPort;
    //    private static string Url => ProfileManager.ServerInfo.Servers["Android"][ProfileManager.Current.ToString()].WebServerIp;
        private static string Url => AkaConfig.Config.BattleServerConfig.GameServer.ip;

        private static string Uri => $"http://{Url}:{Port}/";


        private async static Task<byte[]> RequestAsync<T>(T protoData) where T : BaseProtocol
        {
            try
            {
                WebServerRequestor webServer = new WebServerRequestor();
                return await webServer.RequestAsync(protoData.MessageType, AkaSerializer.AkaSerializer<T>.Serialize(protoData), Uri);
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private static T ToProto<T>(this byte[] rawData)
        {
            return rawData == null ? default(T) : AkaSerializer.AkaSerializer<T>.Deserialize(rawData);
        }

        public async static Task<ProtoOnLogin> Login(string socialAccount)
        {
            var rawData = await RequestAsync(new ProtoLogin
            {
                MessageType = MessageType.Login,
                SocialAccount = socialAccount
            });
            return rawData.ToProto< ProtoOnLogin>();
        }

        internal async static Task SetQuestForComplete(UserInfo userInfo, List<uint> allQuest)
        {
            var questList = allQuest.Select(questId =>
           {
               var quest = Data.GetQuest(questId).First();
               return new ProtoQuestInfo
               {
                   PerformCount = quest.QuestConditionValue,
                   QuestGroupId = questId
               };
           }).ToList();
            _ = RequestAsync(new ProtoSetQuestList
            {
                MessageType = MessageType.SetQuest,
                UserId = userInfo.LoginInfo.UserId,
                QuestInfoList = questList
            });
        }

        internal async static Task<ProtoOnLogin> Join(string name)
        {
            var socialAccount = Utility.UniqueId() + "@" + "akastudiogroup.com";
            var rawData = await RequestAsync(new ProtoAccountJoin
            {
                MessageType = MessageType.AccountJoin,
                NickName = name,
                SocialAccount = socialAccount,
                NightPushAgree = 1,
                LanguageType = "KR",
                PushAgree = 1,
                PlatformType = PlatformType.Google,
                TermsAgree = 1
            });

            return rawData.ToProto<ProtoOnLogin>();

        }

        public async static Task<ProtoOnGetDeck> GetDeck(UserInfo userInfo, ModeType deckModeType)
        {
            var rawData = await RequestAsync( new ProtoGetDeck
            {
                MessageType = MessageType.GetDeck,
                UserId = userInfo.users.userId,
                ModeType = deckModeType
            });
            return rawData.ToProto<ProtoOnGetDeck>();
        }

        public async static Task<ProtoOnGetDeckWithDeckNum> GetDeckWithNum(UserInfo userInfo, ModeType deckModeType)
        {
            var rawData = await RequestAsync(new ProtoGetDeckWithDeckNum
            {
                MessageType = MessageType.GetDeckWithDeckNum,
                UserIdAndDeckNums = new List<KeyValuePair<uint, byte>>()
                {
                    new KeyValuePair<uint, byte>( userInfo.accounts.userId,(byte)userInfo.CurrentDeckNum)
                },
                BattleType = BattleType.LeagueBattle,
                ModeType = deckModeType
            });
            return rawData.ToProto<ProtoOnGetDeckWithDeckNum>();

        }

        public async static Task<ProtoOnGetBattleServer> GetBattleServer()
        {
            var rawData = await RequestAsync(new ProtoGetBattleServer
            {
                MessageType = MessageType.GetBattleServer,
                GroupCode = 1
            });
            return rawData.ToProto<ProtoOnGetBattleServer>();
        }


        internal async static Task SetDeck(UserInfo userInfo, List<ProtoDeckElement> updateDeck)
        {
            var _ = await RequestAsync(new ProtoSetDeck
            {
                MessageType = MessageType.SetDeck,
                UserId = userInfo.LoginInfo.UserId,
                UpdateDecks = updateDeck
            });
        }
    }
}
