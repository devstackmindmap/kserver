
using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaSerializer;
using AkaUtility;
using Common;
using CommonProtocol;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities.SquareObject
{
    public class SquareObjectIO : IContents
    {
        private DBContext _db;
        private DBContext _accountDb;

        public SquareObjectIO()
        {
        }

        public SquareObjectIO(DBContext db, DBContext accountDb)
        {
            _db = db;
            _accountDb = accountDb;
        }



        public async Task<bool> Start(SquareObjectWork squareObjectWork)
        {
            ProtoSquareObjectState squareObject = squareObjectWork.State;
            var isActivated = squareObject.IsActivated ? 1 : 0;
            var activatedTime = squareObject.ActivatedTime.ToTimeString();
            var nextInvasionTime = squareObject.NextInvasionTime.ToTimeString();
            var powerRefreshTime = squareObject.PowerRefreshTime.ToTimeString();
            var energyRefreshRefreshTime = squareObject.EnergyRefreshTime.ToTimeString();

            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET enableReward = 0, isActivated = ").Append(isActivated).Append(", activatedTime='").Append(activatedTime).Append("', nextInvasionTime='").Append(nextInvasionTime)
                 .Append("', nextInvasionLevel=").Append(squareObject.NextInvasionLevel).Append(", nextInvasionMonsterId=").Append(squareObject.NextInvasionMonsterId).Append(", powerRefreshTime ='").Append(powerRefreshTime).Append("', objectPower=").Append(squareObject.SquareObjectPower)
                 .Append(", activeObjectLevel=").Append(squareObject.SquareObjectLevel).Append(", objectShield =").Append(squareObject.CurrentShield).Append(", planetBoxExp=").Append(squareObject.CurrentPlanetBoxExp)
                 .Append(", planetBoxLevel= ").Append(squareObject.CurrentPlanetBoxLevel).Append(", coreEnergy =").Append(squareObject.CoreEnergy)
                 .Append(", energyRefreshTime ='").Append(energyRefreshRefreshTime)
                 .Append("', invasionHistory = @invasionHistory  WHERE userId =").Append(squareObjectWork.UserId).Append(";");


            var paramData = HistoryToSqlParameter(squareObjectWork);
            if (1 == await _db.ExecuteNonQueryAsync(query.ToString(), new MySqlParameter[] { paramData }))
            {
                query.Clear();
                query.Append("UPDATE accounts SET isActivatedSO = 1 WHERE userId = ").Append(squareObject.UserId).Append(";");
                return 1 == await _accountDb.ExecuteNonQueryAsync(query.ToString());
            }
            return false;
        }


        public async Task<bool> AddExtraCoreEnergy(uint userId, int addEnergy, DateTime injectedTime, DateTime initDateTime)
        {
            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET extraCoreEnergy = extraCoreEnergy + ")
                 .Append(addEnergy)
                 .Append(", extraEnergyInjectedTime = '").Append(injectedTime.ToTimeString())
                 .Append("' WHERE userId =").Append(userId).Append(" AND extraEnergyInjectedTime < '")
                 .Append(initDateTime.ToTimeString())
                 .Append("';");
            return 1 == await _db.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<bool> Stop(ProtoSquareObject objectInfo)
        {
            var utcnow = DateTime.UtcNow.ToTimeString();
            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET isActivated = 0, enableReward = 0, objectExp =")
                 .Append(objectInfo.SquareObjectExp)
                 .Append(", energyRefreshTime = '").Append(utcnow).Append("' , nextInvasionTime ='").Append(utcnow)
                 .Append("' WHERE userId = ").Append(objectInfo.CurrentState.UserId).Append(";");

            return 1 == await _db.ExecuteNonQueryAsync(query.ToString()) && await StopAtAccountDB(objectInfo, false);
        }

        public async Task<bool> StopAtAccountDB(ProtoSquareObject objectInfo, bool isDestroyed)
        {
            var query = new StringBuilder();
            query.Append("UPDATE accounts SET isActivatedSO = 0 WHERE userId = ").Append(objectInfo.CurrentState.UserId).Append(";");
            return 1 == await _accountDb.ExecuteNonQueryAsync(query.ToString());
        }

        public async Task<(RewardResultType result, List<ProtoItemResult> rewardItems)> GetRewards(uint userId)
        {
            var squareObject = await SelectSquareObjectWithState(userId);
            if (squareObject.CurrentState.IsActivated == false && squareObject.CurrentState.EnableReward == true)
            {
                var planetBox = Data.GetSquareObjectPlanetBox(squareObject.CurrentState.SquareObjectLevel, squareObject.CurrentState.CurrentPlanetBoxLevel);
                var gettingSquareObjectExp = planetBox.GiveToLoseSquareObjectExp;
                squareObject.SquareObjectExp += gettingSquareObjectExp;

                var query = new StringBuilder();
                query.Append("UPDATE square_object_schedule SET enableReward = 0, objectExp = ").Append(squareObject.SquareObjectExp)
                     .Append(" WHERE userId = ").Append(userId)
                     .Append(" AND isActivated = 0 AND enableReward = 1;");
                await _db.ExecuteNonQueryAsync(query.ToString());

                var itemResult = await GetRewards(squareObject);
                return (RewardResultType.Success, itemResult);
            }
            return (RewardResultType.None, new List<ProtoItemResult>());
        }

        public async Task<List<ProtoItemResult>> GetRewards(ProtoSquareObject objectInfo)
        {
            var itemResult = new List<ProtoItemResult>();
            var objectLevel = objectInfo.CurrentState.SquareObjectLevel;
            var boxLevel = objectInfo.CurrentState.CurrentPlanetBoxLevel;

            var planetBox = Data.GetSquareObjectPlanetBox(objectLevel, boxLevel);
            var rewardId = objectInfo.CurrentState.CurrentShield <= 0 ? planetBox.DestroyRewardId : planetBox.RewardId;
            if (rewardId != 0)
            {
                var rewards = await Reward.Reward.GetRewards(_db, objectInfo.CurrentState.UserId, rewardId, "SquareObjectReward");
                itemResult.AddRange(rewards);
            }

            AkaLogger.Log.User.SquareObject.RewardLog(objectInfo.CurrentState.CurrentShield <= 0, objectInfo.CurrentState.UserId, objectInfo.CurrentState.ActivatedTime, DateTime.UtcNow, objectInfo.CurrentState.NextInvasionTime, objectInfo.CurrentState.NextInvasionLevel, objectInfo.CurrentState.NextInvasionMonsterId
                                                    , objectInfo.CurrentState.CurrentPlanetBoxLevel, rewardId, objectInfo.CurrentState.SquareObjectPower, objectInfo.CurrentState.CurrentShield
                                                    , objectInfo.CurrentState.CurrentShield <= 0, objectInfo.CurrentState.SquareObjectLevel, objectInfo.SquareObjectLevel, objectInfo.CoreLevel, objectInfo.AgencyLevel);
            return itemResult;
        }

        public async Task<ProtoSquareObject> SelectSquareObjectWithState(uint userId)
        {
            var query = new StringBuilder();
            query.Append("SELECT  userId, isActivated, activatedTime, nextInvasionTime, nextInvasionLevel, nextInvasionMonsterId, objectPower, powerRefreshTime, "
                        + " activeObjectLevel, objectShield, planetBoxExp, invasionHistory, coreEnergy, energyRefreshTime, planetBoxLevel,"
                        + " coreLevel, coreExp, agencyExp, agencyLevel, objectLevel, objectExp, enableContents,"
                        + " extraCoreEnergy, extraEnergyInjectedTime, enableReward "
                        + " FROM square_object_schedule WHERE userId = ")
                 .Append(userId)
                 .Append(";");

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {

                if (true == cursor.Read())
                {
                    var enableContents = (uint)cursor["enableContents"];
                    if (enableContents > 0)
                    {
                        var squareObject = GetSquareObject(cursor);
                        return squareObject;
                    }
                }

                return new ProtoSquareObject
                {
                    CurrentState = new ProtoSquareObjectState
                    {
                        InvasionHistory = new List<ProtoSquareObjectInvasionHistory>()
                    }
                };
            }
        }

        public async Task<int> GetDonationCount(uint sendUserId)
        {
            var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
            var utcNow = DateTime.UtcNow.AddHours(-refreshBaseHour);
            var initDateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);

            var query = new StringBuilder();
            query.Append("SELECT COUNT(receivedUserId) FROM square_object_friends WHERE sendUserId = ").Append(sendUserId)
                 .Append(" AND receivedDate >= '").Append(initDateTime.ToTimeString()).Append("';");

            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                if (true == cursor.Read())
                    return cursor.GetInt32(0);
            }
            return 0;
        }


        public async Task<SquareObjectResponseType> DonatePower(uint receivedUserId, uint sendUserId)
        {
            var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
            var utcNow = DateTime.UtcNow.AddHours(-refreshBaseHour);
            var initDateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc).ToTimeString();

            var query = new StringBuilder();
            query.Append("SELECT sendUserId FROM square_object_friends WHERE receivedUserId = ").Append(receivedUserId)
                 .Append(" AND sendUserId =").Append(sendUserId)
                 .Append(" AND receivedDate >= '").Append(initDateTime)
                 .Append("' LIMIT 1;");

            var disableUpdate = true;
            using (var cursor = await _accountDb.ExecuteReaderAsync(query.ToString()))
            {
                disableUpdate = cursor.Read();
            }

            if (disableUpdate)
                return SquareObjectResponseType.AlreadyDonated;

            query.Clear();
            query.Append("UPDATE square_object_friends SET sendUserId = ").Append(sendUserId)
                 .Append(", receivedDate = '").Append(DateTime.UtcNow.ToTimeString())
                 .Append("' WHERE receivedUserId = ").Append(receivedUserId)
                 .Append(" AND receivedDate < '").Append(initDateTime)
                 .Append("' LIMIT 1;");
            return 1 == await _accountDb.ExecuteNonQueryAsync(query.ToString()) ? SquareObjectResponseType.Success : SquareObjectResponseType.MaxHelped;
        }

        public async Task AddExtraCoreEnergy(uint userId)
        {
            var gettingEnergy = (int)(float.Epsilon + Data.GetConstant(DataConstantType.SQUARE_OBJECT_GETTING_ENERGY_DONATION_VALUE).Value);
            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET extraCoreEnergy = extraCoreEnergy + ").Append(gettingEnergy)
                 .Append(" WHERE isActivated = 1 AND userId =").Append(userId).Append(";");
            await _db.ExecuteNonQueryAsync(query.ToString());
        }



        public async Task<bool> UpdateEnergy(ProtoSquareObject squareObject, bool isCompletePlanetBox, DateTime oldEnergyRefreshTime)
        {
            var squareObjectState = squareObject.CurrentState;

            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET planetBoxExp = ").Append(squareObjectState.CurrentPlanetBoxExp)
                 .Append(" , coreEnergy = ").Append(squareObjectState.CoreEnergy)
                 .Append(" , objectPower = ").Append(squareObjectState.SquareObjectPower)
                 .Append(" , planetBoxLevel = ").Append(squareObjectState.CurrentPlanetBoxLevel)
                 .Append(" , extraCoreEnergy = ").Append(squareObjectState.ExtraCoreEnergy)
                 .Append(" , energyRefreshTime = '").Append(squareObjectState.EnergyRefreshTime.ToTimeString())
                 .Append("' , powerRefreshTime = '").Append(squareObjectState.PowerRefreshTime.ToTimeString())
                 .Append("' WHERE isActivated = 1 AND userId = ").Append(squareObjectState.UserId)
                 .Append(" AND energyRefreshTime = '").Append(oldEnergyRefreshTime.ToTimeString()).Append("'; ");
            return 1 == await _db.ExecuteNonQueryAsync(query.ToString());
        }


        public async Task<bool> UpdateInvasionResult(SquareObjectWork work, DateTime firstInvasionTime)
        {
            var isActivated = work.State.IsActivated ? 1 : 0;
            var enableReward = work.State.EnableReward ? 1 : 0;
            var nextInvasionTime = work.State.NextInvasionTime;
            var nextInvasionLevel = work.State.NextInvasionLevel;
            var nextInvasionMonsterId = work.State.NextInvasionMonsterId;

            var squarePower = work.State.SquareObjectPower;
            var powerRefreshTime = work.State.PowerRefreshTime;
            var squareShield = work.State.CurrentShield;
            var coreExp = work.ObjectInfo.CoreExp + work.State.InvasionHistory.Where(history => history.GettingCoreExp != 0).Select(history =>
            {
                var exp = history.GettingCoreExp;
                history.GettingCoreExp = 0;
                return exp;
            }).Sum(exp => exp);
            var agencyExp = work.ObjectInfo.AgencyExp + work.State.InvasionHistory.Where(history => history.GettingAgencyExp != 0).Select(history =>
            {
                var exp = history.GettingAgencyExp;
                history.GettingAgencyExp = 0;
                return exp;
            }).Sum(exp => exp);

            work.ObjectInfo.CoreExp = (int)coreExp;
            work.ObjectInfo.AgencyExp = (int)agencyExp;

            var serializedInvasionHistory = work.InvasionHistoryToBinary();
            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET isActivated = ").Append(isActivated)
                 .Append(", nextInvasionTime = '").Append(nextInvasionTime.ToTimeString())
                 .Append("', nextInvasionLevel = ").Append(nextInvasionLevel)
                 .Append(" , nextInvasionMonsterId = ").Append(nextInvasionMonsterId)
                 .Append(" , objectPower =").Append(squarePower)
                 .Append(" , powerRefreshTime ='").Append(powerRefreshTime.ToTimeString())
                 .Append("', objectShield =").Append(squareShield)
                 .Append(" , coreExp = ").Append(coreExp)
                 .Append(" , agencyExp = ").Append(agencyExp)
                 .Append(" , enableReward = ").Append(enableReward)
                 .Append(" , invasionHistory = @invasionHistory WHERE userId = ").Append(work.UserId)
                 .Append(" AND nextInvasionTime = '").Append(firstInvasionTime.ToTimeString()).Append("'; ");

            var paramData = HistoryToSqlParameter(work);
            var updatedCount = await _db.ExecuteNonQueryAsync(query.ToString(), new MySqlParameter[] { paramData });
            if (updatedCount != 1)
                return false;

            //update destroy reward
            if (work.State.IsActivated == false)
            {
                await StopAtAccountDB(work.ObjectInfo, true);
            }
            return true;

        }

        private MySqlParameter HistoryToSqlParameter(SquareObjectWork work)
        {
            var serializedInvasionHistory = work.InvasionHistoryToBinary();
            var paramData = new MySqlParameter("@invasionHistory", MySqlDbType.Blob, serializedInvasionHistory.Length);
            paramData.Value = serializedInvasionHistory;
            return paramData;
        }

        public ProtoSquareObjectState GetSquareObjectState(DbDataReader cursor)
        {
            var squareObjectState = new ProtoSquareObjectState
            {
                UserId = (uint)cursor["userId"],
                IsActivated = 0 != (uint)cursor["isActivated"],
                NextInvasionLevel = (uint)cursor["nextInvasionLevel"],
                NextInvasionMonsterId = (uint)cursor["nextInvasionMonsterId"],
                ActivatedTime = new DateTime(((DateTime)cursor["activatedTime"]).Ticks, DateTimeKind.Utc),
                NextInvasionTime = new DateTime(((DateTime)cursor["nextInvasionTime"]).Ticks, DateTimeKind.Utc),

                SquareObjectPower = (int)cursor["objectPower"],
                PowerRefreshTime = new DateTime(((DateTime)cursor["powerRefreshTime"]).Ticks, DateTimeKind.Utc),
                SquareObjectLevel = (uint)cursor["activeObjectLevel"],
                CurrentShield = (int)cursor["objectShield"],
                CurrentPlanetBoxExp = (int)cursor["planetBoxExp"],
                CurrentPlanetBoxLevel = (uint)cursor["planetBoxLevel"],
                CoreEnergy = (int)cursor["coreEnergy"],
                EnergyRefreshTime = new DateTime(((DateTime)cursor["energyRefreshTime"]).Ticks, DateTimeKind.Utc),
                ExtraCoreEnergy = (int)cursor["extraCoreEnergy"],
                ExtraEnergyInjectedTime = new DateTime(((DateTime)cursor["extraEnergyInjectedTime"]).Ticks, DateTimeKind.Utc),
                EnableReward = 0 != (int)cursor["enableReward"]
            };

            var historyOrdinal = cursor.GetOrdinal("invasionHistory");
            if (cursor.IsDBNull(historyOrdinal))
                squareObjectState.InvasionHistory = new List<ProtoSquareObjectInvasionHistory>();
            else
                squareObjectState.InvasionHistory = AkaSerializer.AkaSerializer<List<ProtoSquareObjectInvasionHistory>>.Deserialize((byte[])cursor["invasionHistory"]);
            return squareObjectState;
        }

        public ProtoSquareObject GetSquareObject(DbDataReader cursor)
        {
            return new ProtoSquareObject
            {
                SquareObjectLevel = (uint)cursor["objectLevel"],
                SquareObjectExp = (int)cursor["objectExp"],
                CoreLevel = (uint)cursor["coreLevel"],
                CoreExp = (int)cursor["coreExp"],
                AgencyLevel = (uint)cursor["agencyLevel"],
                AgencyExp = (int)cursor["agencyExp"],
                CurrentState = GetSquareObjectState(cursor)
            };
        }

        public async Task<bool> UnlockContents(uint userId)
        {
            var query = new StringBuilder();
            query.Append("UPDATE square_object_schedule SET enableContents = 1 WHERE userId = ").Append(userId.ToString()).Append(";");
            return 0 < await _db.ExecuteNonQueryAsync(query.ToString());
        }

    }
}
