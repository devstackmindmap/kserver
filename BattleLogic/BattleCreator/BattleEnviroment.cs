using AkaData;
using AkaDB.MySql;
using AkaEnum;
using AkaEnum.Battle;
using CommonProtocol;
using System;
using System.Collections.Generic;
using AkaUtility;

namespace BattleLogic
{
    public class BattleEnviroment
    {
        public DataContentsConstant Enviroment { get;  }

        public BattleEnviroment(BattleType battleType)
        {
            Enviroment = AkaUtility.Utility.CopyFrom(Data.GetContentsConstant(battleType));
        }
        
        public ModeType DeckModeType => Enviroment.DeckModeType;

        public BattleType BattleType => Enviroment.BattleType;
        public double BattleTimeMillisec => Enviroment.BattleTime * 1000;
        public double BattleBoosterTimeMillisec => Enviroment.BoosterTime * 1000;
        public double CanRetreatTimeSec => Enviroment.CanRetreatTime;
        public double BattleExtensionTime => Enviroment.ExtensionTime;
        public double ChargingElixirTime => Enviroment.ChargingElixirTime;
        public float BoosterElixirMultiple => Enviroment.BoosterElixirMultiple;
        public float DefaultElixir => Enviroment.DefaultElixir;
        public float MaxElixir => Enviroment.MaxElixir;
        public bool CanEmoticon => Enviroment.CanEmoticon;

        public bool IsPvPBattle()
        {
            return BattleEnviroment.IsPvPBattle( Enviroment.BattleType );
        }

        public static bool IsPvPBattle(BattleType battleType)
        {
            return battleType.In(BattleType.LeagueBattle , 
                                 BattleType.Assault_Arena_High,
                                 BattleType.Assault_Arena_Low,
                                 BattleType.FriendlyBattle);
        }
               
        public static ModeType GetDeckModeType(BattleType battleType)
        {
            return Data.GetContentsConstant(battleType).DeckModeType;
        }
    }
}
