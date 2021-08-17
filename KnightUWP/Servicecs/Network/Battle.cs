using AkaData;
using AkaEnum;
using AkaEnum.Battle;
using AkaSerializer;
using AkaUtility;
using CommonProtocol;
using KnightUWP.Dao;
using KnightUWP.Servicecs.Network;
using KnightUWP.Servicecs.Protocol;
using Microsoft.Toolkit.Uwp.Helpers;
using SuperSocket.ClientEngine;
using System.Linq;
using System.Threading.Tasks;

namespace KnightUWP.Servicecs.Net
{
    static class Battle
    {
        private static readonly byte[] _SerializedSyncTime = AkaSerializer<ProtoSyncTime>.Serialize(new ProtoSyncTime());

        private static void Battle_ConnectedEvent(UserInfo userInfo, bool connected)
        {

            if (userInfo.State.In(UserState.BattleBeforeStart, UserState.BattleStart))
            {
                userInfo.State = UserState.BattleWrongClosed;

                userInfo.WriteLog();
            }
        }

        private static void Battle_DataReceived( UserInfo userInfo, DataEventArgs e )
        {
            if ((e?.Data?.Length ?? 0) == 0)
            {
                return;
            }

            e = new DataEventArgs()
            {
                Data = e.Data.CloneRange(0, e.Data.Length),
                Length = e.Length,
                Offset = e.Offset
            };

            userInfo.BattleDatas.Enqueue(e);
            lock (userInfo)
            {
                if (userInfo.Processor?.IsCompleted ?? true)
                {
                    userInfo.Processor = Task.Factory.StartNew(async () =>
                    {
                        await DoProcess(userInfo);
                    });
                }
            }
        }

        private static async Task DoProcess(UserInfo userInfo)
        {
            while (userInfo.BattleDatas.TryDequeue(out var e))
            {
                await DispatcherHelper.ExecuteOnUIThreadAsync(() =>
                {
                    ProtocolProcessGenerator.DoProcess(userInfo, e);
                }, Windows.UI.Core.CoreDispatcherPriority.Normal);
            }
        }


        public static void EnterRoom(UserInfo userInfo)
        {

            if (userInfo.BattleConnecter?.IsConnected == true)
                userInfo.BattleConnecter.Close();
            userInfo.BattleConnecter?.WaitClose(3000);


            if (userInfo.BattleDatas == null)
                userInfo.BattleDatas = new System.Collections.Concurrent.ConcurrentQueue<DataEventArgs>();
            userInfo.BattleDatas.Clear();

            userInfo.CurrentBattleInfo = new BattleInfo();
            userInfo.CurrentBattleInfo.VisibileHistory = VOProvider.Instance.EnableEnqueueActionLog ? userInfo.CurrentBattleInfo.History : userInfo.CurrentBattleInfo.FilteredHistory;

            userInfo.BattleConnecter = new ClientNetworkConnector($"{userInfo.accounts.socialAccount}_b_connector");
            userInfo.BattleConnecter.Connect(userInfo.CurrentMatchingInfo.BattleServerIp, int.Parse( userInfo.CurrentMatchingInfo.BattleServerPort) , Battle_ConnectedEvent, Battle_DataReceived, userInfo);


            userInfo.State = UserState.BattleEnterRoom;

            if (false == userInfo.BattleConnecter.WaitConnect(30000))
            {
                userInfo.State = UserState.None;
                userInfo.WriteLog("BattleConnectionFailed");

                VOProvider.Instance.IncreaseTotalBattleConnecFailCount();

                return;
            }

            var msg = MessageType.EnterRoom;
            byte[] protocol = null;
            if (userInfo.CurrentMatchingInfo.BattleType == BattleType.LeagueBattleAi)
            {
                msg = MessageType.EnterPveRoom;
                protocol = AkaSerializer<ProtoEnterPveRoom>.Serialize(new ProtoEnterPveRoom
                {
                    MessageType = MessageType.EnterPveRoom,
                    UserId = userInfo.users.userId,
                    StageRoundId = userInfo.CurrentMatchingInfo.StageRoundId,
                    StageLevelId = 0,
                    BattleType = userInfo.CurrentMatchingInfo.BattleType,
                    DeckNum = 0,
                    BattleServerIp = userInfo.CurrentMatchingInfo.BattleServerIp
                });

                userInfo.CurrentBattleInfo.IsPvP = false;
            }
            else if (userInfo.CurrentMatchingInfo.BattleType.In(BattleType.LeagueBattle, BattleType.FriendlyBattle))
            {
                protocol = AkaSerializer<ProtoEnterRoom>.Serialize(new ProtoEnterRoom
                {
                    MessageType = MessageType.EnterRoom,
                    UserId = userInfo.users.userId,
                    RoomId = userInfo.CurrentMatchingInfo.RoomId,
                    BattleType = userInfo.CurrentMatchingInfo.BattleType,
                    DeckNum = 0,
                    BattleServerIp = userInfo.CurrentMatchingInfo.BattleServerIp
                });

                userInfo.CurrentBattleInfo.IsPvP = true;
            }

            Utility.Log($"[EnterRoom] {userInfo.accounts.userId} vs {(userInfo.CurrentBattleInfo.IsPvP ? "Player" : "Ai" )}");
            userInfo.BattleConnecter.Send(msg, protocol);
        }

        public static void SendCardUse(UserInfo userInfo)
        {
            userInfo.CurrentBattleInfo.UseCount = 100;
            if (userInfo.CurrentBattleInfo.NextCardStatId == 0)
            {
                userInfo.CurrentBattleInfo.SetNextCard();
                return;
            }

            var cardStat = Data.GetCardStat(userInfo.CurrentBattleInfo.NextCardStatId);
            var cardUnitId = Data.GetCard(cardStat.CardId).UnitId;
            var performUnits = userInfo.CurrentBattleInfo.MyUnits.Select((unit, index) => unit.UnitId != cardUnitId || unit.IsDeath ? 99 : index).Where(index => index != 99);

            var unitposition = performUnits.Any() ? performUnits.First() : -1;

            if (unitposition == -1)
            {
                userInfo.CurrentBattleInfo.SetNextCard();
                return;
            }


            TargetGroupType targetGroup;
            TargetType targetType;
            if (0 != cardStat.SkillConditionId)
            {
                var condition = Data.GetSkillCondition(cardStat.SkillConditionId);
                targetGroup = condition.TargetGroupType;
                targetType = condition.TargetType;
            }
            else
            {
                var skill = Data.GetSkillWithoutAnimationData(cardStat.SkillIdList[0]);
                targetGroup = TargetGroupType.Enemy;//skill.TargetGroupType;
                targetType = TargetType.Target;//skill.TargetType;

            }

            int targetIndex = 0;
            if (targetType == TargetType.Self)
            {
                targetType = TargetType.Target;
                targetIndex = unitposition;
            }
            else if (targetType == TargetType.Target)
            {
                targetGroup = targetGroup == TargetGroupType.All ? (TargetGroupType)AkaRandom.Random.NextUint(1, 3) : targetGroup;
                var targets = targetGroup == TargetGroupType.Ally ? userInfo.CurrentBattleInfo.MyUnits : userInfo.CurrentBattleInfo.EnemyUnits;
                var units = userInfo.CurrentBattleInfo.MyUnits.Select((unit, index) => unit.IsDeath ? 99 : index).Where(index => index != 99);
                if (units.Any())
                {
                    targetIndex = AkaRandom.Random.ChooseIndexRanddomlyInSourceByCount(units, 1).First();
                }
            }

            var cardUse = new ProtoCardUse
            {
                MessageType = MessageType.CardUse,
                RoomId = userInfo.CurrentBattleInfo.BeforeBattleStartInfo.RoomId,
                HandIndex = (int)userInfo.CurrentBattleInfo.NextCard,
                Performer = new ProtoTarget
                {
                    PlayerType = userInfo.CurrentBattleInfo.MyPlayer,
                    UnitPositionIndex = unitposition
                },
                Target = new ProtoTarget
                {
                    PlayerType = targetType != TargetType.Target ? PlayerType.All : targetGroup == TargetGroupType.Ally ? userInfo.CurrentBattleInfo.MyPlayer : userInfo.CurrentBattleInfo.EnemyPlayer,
                    UnitPositionIndex = targetIndex
                }
            };
            userInfo.BattleConnecter.Send(MessageType.CardUse, AkaSerializer<ProtoCardUse>.Serialize(cardUse) );

            DispatcherHelper.ExecuteOnUIThreadAsync(() =>
            {
                new SendCardUse().OnPreResponse(userInfo, cardUse);
            });
        }

        public static void SendReady(UserInfo userInfo)
        {
            var protoBattleReady = new ProtoBattleReady()
            {
                MessageType = MessageType.BattleReady,
                RoomId = userInfo.CurrentBattleInfo.BeforeBattleStartInfo.RoomId,
                UserId = userInfo.users.userId
            };
            userInfo.BattleConnecter.Send(MessageType.BattleReady, AkaSerializer<ProtoBattleReady>.Serialize(protoBattleReady));
        }

        public static void SendSyncTime(UserInfo userInfo)
        {
            userInfo.BattleConnecter.Send(MessageType.SyncTime, _SerializedSyncTime);
        }
    }
}
