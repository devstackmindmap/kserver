using AkaDB.MySql;
using AkaUtility;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Entities.Reward
{
    public class ProbabilityGroup
    {
        private uint _userId;
        private uint _classId;
        private DBContext _db;

        public ProbabilityGroup(uint userId, uint classId, DBContext db)
        {
            _userId = userId;
            _classId = classId;
            _db = db;
        }

        public uint GetBox()
        {
            var probabilityGroup = AkaData.Data.GetProbabilityGroup(_classId);
            var selectedIndex = AkaRandom.Random.ChooseIndexRandomlyInSumOfProbability(probabilityGroup);
            return probabilityGroup[selectedIndex].ElementId;
        }

        public uint GetBoxWithCorrection(IDictionary<uint, int> corrections)
        {
            var probabilityGroup = AkaData.Data.GetProbabilityGroup(_classId);
            var selectedIndex = AkaRandom.Random.ChooseIndexRandomlyInSumOfProbabilityWithCorrection(probabilityGroup, corrections);
            return probabilityGroup[selectedIndex].ElementId;
        }
    }
}
