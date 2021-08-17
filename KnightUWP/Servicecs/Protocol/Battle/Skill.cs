using AkaData;
using AkaDB.MySql;
using AkaSerializer;
using CommonProtocol;
using KnightUWP.Dao;
using KnightUWP.Dao.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace KnightUWP.Servicecs.Protocol
{
    [Message(MessageType = MessageType.Skill)]
    public class Skill : AbstractBattleProcess
    {
        public override BaseProtocol OnResponseDataToProtocol<TContext>(TContext context, byte[] data)
        {
            return AkaSerializer<ProtoSkill>.Deserialize(data);
        }

        
        public override void OnResponse<TContext>(TContext context, BaseProtocol _ )
        {
            var userInfo = context as UserInfo;
            var protocol = Event as ProtoSkill;

            ObservableCollection<UnitInfo> performUnits = userInfo.CurrentBattleInfo.MyPlayer == protocol.PerformerPlayerType ?
                                                            userInfo.CurrentBattleInfo.MyUnits : userInfo.CurrentBattleInfo.EnemyUnits;
            var performUnit = performUnits.FirstOrDefault(unit => unit.UnitId == protocol.PerformerUnitId);
            
            var dataSkill = Data.GetDataSkill(protocol.SkillId);

            if (performUnit != null)
            {
                var bulletTime = Data.GetSkill(protocol.SkillId, performUnit.Skin).AnimationData.BulletTime;
                if (bulletTime != 0)
                    userInfo.CurrentBattleInfo.AddBulletTime(bulletTime);
            }
            //TODO 
            // change unit status, hp takedamage
            //enqueue actor queue
            //bullet time

            if (protocol.ReplacedCardStatId == 0 || protocol.PerformerPlayerType != userInfo.CurrentBattleInfo.MyPlayer)
                return;
           
            switch (protocol.ReplacedHandIndex)
            {
                case 0:
                    userInfo.CurrentBattleInfo.Card1 = protocol.ReplacedCardStatId; break;
                case 1:
                    userInfo.CurrentBattleInfo.Card2 = protocol.ReplacedCardStatId; break;
                case 2:
                    userInfo.CurrentBattleInfo.Card3 = protocol.ReplacedCardStatId; break;
                case 3:
                    userInfo.CurrentBattleInfo.Card4 = protocol.ReplacedCardStatId; break;
            }
            userInfo.CurrentBattleInfo.SetNextCard();

        }
        public override string OutLogOnResponse<TContext>(TContext context)
        {
            var protocol = Event as ProtoSkill;
            string targetLog = string.Join("\n\t\t\t", protocol.SkillOptionResults.Select(skillOption => $"SkillOption:{skillOption.SkillOptionId} Targets:{string.Join(",", skillOption.SkillTargetResults.Select(skillTarget => skillTarget.TargetUnitId)) } "));

            return $"{protocol.PerformerPlayerType}:{protocol.PerformerUnitId}-Skill{protocol.SkillId} ReplaceCard{protocol.ReplacedHandIndex}:{protocol.ReplacedCardStatId} Next{protocol.NextCardStatId} \n\t\t\t{targetLog}";
        }

        protected override HorizontalAlignment GetPerformer(BaseProtocol protocol)
        {
            var skill = protocol as ProtoSkill;
            if (skill.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
                return HorizontalAlignment.Left;
            else
                return HorizontalAlignment.Right;
        }

        protected override string GetLeftSide(BaseProtocol protocol)
        {
            var skill = protocol as ProtoSkill;
            if (skill.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(skill.PerformerUnitId).UnitInitial}({skill.PerformerUnitId}) - {TextGetter.GetCardText(skill.UsedCardStatId)}({skill.UsedCardStatId})";
            }
            else
            {
                var targetResults =
                skill.SkillOptionResults
                    .SelectMany(skillOptionResult => skillOptionResult.SkillTargetResults)
                    .Select(skillTargetResult => {
                        return $"{(skillTargetResult.TargetPlayerType == skill.PerformerPlayerType ? "나" : "적")}:{Data.GetUnit(skillTargetResult.TargetUnitId).UnitInitial}({skillTargetResult.TargetUnitId})";                        
                    });
                return string.Join(", ", targetResults);                
            }
        }

        protected override string GetRightSide(BaseProtocol protocol)
        {
            var skill = protocol as ProtoSkill;
            if (skill.PerformerPlayerType != UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"{Data.GetUnit(skill.PerformerUnitId).UnitInitial}({skill.PerformerUnitId}) - {TextGetter.GetCardText(skill.UsedCardStatId)}({skill.UsedCardStatId})";
            }
            else
            {
                var targetResults =
                skill.SkillOptionResults
                    .SelectMany(skillOptionResult => skillOptionResult.SkillTargetResults)
                    .Select(skillTargetResult => {
                        return $"{(skillTargetResult.TargetPlayerType == skill.PerformerPlayerType ? "나" : "적")}:{Data.GetUnit(skillTargetResult.TargetUnitId).UnitInitial}({skillTargetResult.TargetUnitId})";
                    });
                return string.Join(", ", targetResults);
            }
        }

        protected override string GetCenter(BaseProtocol protocol)
        {
            var skill = protocol as ProtoSkill;
            if (skill.PerformerPlayerType == UserInfo.CurrentBattleInfo.MyPlayer)
            {
                return $"  ======  ";
            }
            else
            {
                return $"  ======  ";
            }
        }
    }
}
