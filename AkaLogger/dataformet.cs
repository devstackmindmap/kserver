using System;

namespace AkaLogger
{
    public class Skillcard
    {
        public string cardID { get; set; }
        public int level { get; set; }
    }

    public class Unit
    {
        public string unitID { get; set; }
        public int Level { get; set; }
        public string Weapon { get; set; }
    }

    public class Buff
    {
        public string buffSkillID { get; set; }
        public string buffcondition { get; set; }

    }

    public class Skilluse
    {
        public string battleServerNum { get; set; }
        public string RoomID { get; set; }
        public string sourceUserID { get; set; }
        public string sourceNickname { get; set; }
        public string TargetUserID { get; set; }
        public string TargetNicknama { get; set; }
        public string attackUnitID { get; set; }
        public string SkillID { get; set; }
        public int Cost { get; set; }
        public int BeforeCost { get; set; }
        public int AfterCost { get; set; }
        public bool TargetType { get; set; } // 0: Allay 1: enermy
        public string TargetunitID { get; set; }
    }

    public class SkilluseResult
    {
        public string battleServerNum { get; set; }
        public string RoomID { get; set; }
        public string sourceUserID { get; set; }
        public string sourceNickname { get; set; }
        public string TargetUserID { get; set; }
        public string TargetNicknama { get; set; }
        public string attackUnitID { get; set; }
        public string SkillID { get; set; }
        
        public bool TargetType { get; set; } // 0: Allay 1: enermy
        public string TargetunitID { get; set; }
    }

    public class retreat
    {
        public string battleServerNum { get; set; }
        public string RoomID { get; set; }
        public string UserID { get; set; }
        public string Nickname { get; set; }
    }

}
