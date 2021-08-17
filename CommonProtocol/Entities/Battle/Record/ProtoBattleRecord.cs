using AkaEnum.Battle;
using MessagePack;
using System;
using System.Collections.Generic;

namespace CommonProtocol
{
    [MessagePackObject]
    public class ProtoBattleRecord : BaseProtocol
    {
        [Key(1)]
        public uint Seq;

        [Key(2)]
        public BattleType BattleType;

        [Key(3)]
        public uint UserId;

        [Key(4)]
        public uint EnemyUserId;

        [Key(5)]
        public long BattleStartTime;

        [Key(6)]
        public long BattleEndTime;

        [Key(7)]
        public List<ProtoBattleRecordBehavior> Behaviors;

        [Key(8)]
        public ProtoBattleRecordInfo BattleInfo;

        [Key(9)]
        public bool IsHost;
        
        [Key(10)]
        public string RecordKey;


    }
}