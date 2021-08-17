using AkaEnum;

namespace AkaData
{
    public class DataUnit
    {
        public uint UnitId { get; set; }
        public string UnitInitial { get; set; }
        public uint SkinGroupId { get; set; }
        public UnitType UnitType { get; set; }
        public UserType UserType { get; set; }
        public float AttackRange { get; set; }
        public float UnitMoveSpeed { get; set; }
        public GetUnitType GetUnitType { get; set; }
    }
}
