
using AkaDB.MySql;
using MySql.Data.MySqlClient;
using AkaEnum;
using CommonProtocol;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AkaUtility;
using AkaEnum.Battle;
using AkaSerializer;
using System;
using System.Text;
using AkaConfig;

namespace Common.Entities.Stage
{
    public class DBBattleRecord
    {
        private DBContext _db;


        public DBBattleRecord(DBContext db)
        {
            _db = db;
        }

        public async Task SaveRecord(ProtoBattleRecord record)
        {
            var battleInfoData = AkaSerializer<ProtoBattleRecordInfo>.Serialize(record.BattleInfo);

            var paramInfo = new DBParamInfo();
            paramInfo.SetInputParam(
                new InputArg("$battleType", record.BattleType.ToString()),
                new InputArg("$userId", record.UserId.ToString()),
                new InputArg("$enemyUserId", record.EnemyUserId.ToString()),
                new InputArg("$battleStartTime", new DateTime(record.BattleStartTime).ToTimeString()),
                new InputArg("$battleEndTime", new DateTime(record.BattleEndTime).ToTimeString()),
                new InputArg("$battleRecordKey", record.RecordKey),
                new InputArg("$battleInfo", battleInfoData),
                new InputArg("$isHost", record.IsHost ? "1" : "0")
                );
            paramInfo.SetOutputParam(new OutputArg("$outPlayerSeq", MySql.Data.MySqlClient.MySqlDbType.UInt32));
            await _db.CallStoredProcedureAsync(StoredProcedure.ADD_BATTLE_RECORD, paramInfo);
        }


        public async Task<ProtoOnGetBattleRecordList> GetRecordList(ProtoGetBattleRecordList reqParam)
        {
            var battleTypeCondition = string.Join(",", reqParam.BattleTypeList.Select(battleType => $"'{battleType.ToString()}'") );
            var limit = reqParam.BattleTypeList.Max(battleType => AkaData.Data.GetContentsConstant(battleType).BattleRecordCount);
            var lifeTime = reqParam.BattleTypeList.Max(battleType => AkaData.Data.GetContentsConstant(battleType).BattleRecordLifeTime);
             
            StringBuilder query = new StringBuilder();
            query.Append("SELECT `seq`, `recordKey`, `battleType`, `userId`, `enemyUserId`, `battleStartTime`, `battleEndTime`, `battleInfo`, `isHost`  ")
                .Append("FROM battle_records ")
                .Append("WHERE `userId` = ").Append(reqParam.UserId)
                .Append(" AND `battleType` IN (").Append(battleTypeCondition).Append(") ");

            if (lifeTime > 0)
            {
                var now = DateTime.Now.AddDays(-lifeTime);
                query.Append(" AND battleEndTime > '").Append(now.ToString("yyyy-MM-dd")).Append("' ");
            }

            if (limit > 0)
                query.Append("ORDER BY seq DESC LIMIT ").Append(limit.ToString()); 
            query.Append(";");

            List<ProtoBattleRecord> recordList = new List<ProtoBattleRecord>();

            using (var cursor = await _db.ExecuteReaderAsync(query.ToString()))
            {
                while(cursor.Read())
                {
                    var protoBattleRecord = FromDbReader(cursor);
                    protoBattleRecord.MessageType = MessageType.SaveBattleRecordInfo;
                    recordList.Add(protoBattleRecord);
                }
            }

            return new ProtoOnGetBattleRecordList
            {
                MessageType = MessageType.GetBattleRecordList,
                BattleRecordList = recordList
            };
        }
        
        private ProtoBattleRecord FromDbReader(System.Data.Common.DbDataReader cursor)
        {
            var protoBattleRecord = new ProtoBattleRecord();
            protoBattleRecord.Seq = (uint)cursor["seq"];
            protoBattleRecord.RecordKey = (string)cursor["recordKey"];
            protoBattleRecord.UserId = (uint)cursor["userId"];
            protoBattleRecord.EnemyUserId = (uint)cursor["enemyUserId"];
            protoBattleRecord.BattleType = (cursor["battleType"] as string).CastToEnum<BattleType>();
            protoBattleRecord.BattleStartTime = ((DateTime)cursor["battleStartTime"]).Ticks;
            protoBattleRecord.BattleEndTime = ((DateTime)cursor["battleEndTime"]).Ticks;
            protoBattleRecord.IsHost = 1 == (uint)cursor["isHost"];

            var battleInfoData = (byte[])cursor["battleInfo"];
            var battleInfo = AkaSerializer<ProtoBattleRecordInfo>.Deserialize(battleInfoData);
            protoBattleRecord.BattleInfo = battleInfo;
            return protoBattleRecord;
        }
    }
}
