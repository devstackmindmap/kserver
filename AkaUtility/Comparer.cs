using AkaEnum;
using System;
using System.Collections.Generic;

namespace AkaUtility
{
    public class DataComparer<AkaDataType> : IEqualityComparer<AkaDataType> where AkaDataType : class
    {
        private Func<AkaDataType, AkaDataType, bool> _comparer = null;
        private Func<AkaDataType, int> _hash = null;
        public DataComparer(Func<AkaDataType, AkaDataType, bool> compareDelegate, Func<AkaDataType, int> hashDelegate)
        {
            _comparer = compareDelegate;
            _hash = hashDelegate;
        }

        public bool Equals(AkaDataType arg1, AkaDataType arg2)
        {
            return _comparer?.Invoke(arg1,arg2) ?? arg1.Equals(arg2);
        }

        public int GetHashCode(AkaDataType obj)
        {
            return _hash?.Invoke(obj) ?? obj.GetHashCode();
        }
    }

    public class DataTypeComparer : IEqualityComparer<DataType>
    {
        public static readonly DataTypeComparer Comparer = new DataTypeComparer();

        public bool Equals(DataType x, DataType y)
        {
            return x == y;
        }

        public int GetHashCode(DataType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class OperationConditionTypeComparer : IEqualityComparer<OperationConditionType>
    {
        public static readonly OperationConditionTypeComparer Comparer = new OperationConditionTypeComparer();

        public bool Equals(OperationConditionType x, OperationConditionType y)
        {
            return x == y;
        }

        public int GetHashCode(OperationConditionType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class SkillEffectTypeComparer : IEqualityComparer<SkillEffectType>
    {
        public static readonly SkillEffectTypeComparer Comparer = new SkillEffectTypeComparer();

        public bool Equals(SkillEffectType x, SkillEffectType y)
        {
            return x == y;
        }

        public int GetHashCode(SkillEffectType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class PlayerTypeComparer : IEqualityComparer<AkaEnum.Battle.PlayerType>
    {
        public static readonly PlayerTypeComparer Comparer = new PlayerTypeComparer();

        public bool Equals(AkaEnum.Battle.PlayerType x, AkaEnum.Battle.PlayerType y)
        {
            return x == y;
        }

        public int GetHashCode(AkaEnum.Battle.PlayerType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class ModeTypeComparer : IEqualityComparer<ModeType>
    {
        public static readonly ModeTypeComparer Comparer = new ModeTypeComparer();

        public bool Equals(ModeType x, ModeType y)
        {
            return x == y;
        }

        public int GetHashCode(ModeType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class SlotTypeComparer : IEqualityComparer<SlotType>
    {
        public static readonly SlotTypeComparer Comparer = new SlotTypeComparer();

        public bool Equals(SlotType x, SlotType y)
        {
            return x == y;
        }

        public int GetHashCode(SlotType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class ServerComparer : IEqualityComparer<Server>
    {
        public static readonly ServerComparer Comparer = new ServerComparer();

        public bool Equals(Server x, Server y)
        {
            return x == y;
        }

        public int GetHashCode(Server obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class RewardCategoryTypeComparer : IEqualityComparer<RewardCategoryType>
    {
        public static readonly RewardCategoryTypeComparer Comparer = new RewardCategoryTypeComparer();

        public bool Equals(RewardCategoryType x, RewardCategoryType y)
        {
            return x == y;
        }

        public int GetHashCode(RewardCategoryType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }

    public class SkillConditionTypeComparer : IEqualityComparer<SkillConditionType>
    {
        public static readonly SkillConditionTypeComparer Comparer = new SkillConditionTypeComparer();

        public bool Equals(SkillConditionType x, SkillConditionType y)
        {
            return x == y;
        }

        public int GetHashCode(SkillConditionType obj)
        {
            return ((int)obj).GetHashCode();
        }
    }
}