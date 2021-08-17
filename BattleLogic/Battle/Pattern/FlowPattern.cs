

using AkaData;
using System.Collections.Generic;
using System.Linq;
using AkaUtility;

namespace BattleLogic
{
    public class FlowPattern : Pattern
    {
        internal Pattern ParentPattern { get; set; }
        internal uint NextPatternId { get; set; }
        internal uint FlowPatternId { get; set; }

        public FlowPattern() : base(1) { }

        internal override bool ActiveConditionsCheck => false;

        internal override bool DeactiveConditionsCheck => false;

        public override bool CanDoAction()
        {
            return ParentPattern.Deactived == false && base.CanDoAction();
        }
        internal override void DoAction()
        {
        }

        internal override void DidAction()
        {
        }
    }
}
