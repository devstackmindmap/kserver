using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquareObjectSimulator.Services
{
    using AkaConfig;
    using AkaData;
    using AkaEnum;
    using Common.Entities.SquareObject;
    using CommonProtocol;
    using DAO;
    using SquareObjectSimulator.VO;




    public class SquareObject
    {
        private Dictionary<int, SquareObjectVO> objects = new Dictionary<int, SquareObjectVO>();

        public static SquareObject Instance { get; private set; }

        static SquareObject()
        {
            Instance = new SquareObject();
        }

        public SquareObject()
        {
        }

        public void Init()
        {
            //TODO load init

        }


        public SquareObjectVO Get(int id)
        {
            return objects.TryGetValue(id, out var obj) ? (SquareObjectVO)obj : new SquareObjectVO();
        }

        public void Set(SquareObjectVO so)
        {
            if (objects.ContainsKey(so.Id))
            {
                objects[so.Id] = so;
            }
        }

        internal SquareObjectVO GetObject(int id)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);

                var firstInvasionTime = TestSquareObjectWork.State.NextInvasionTime;

                var invaded = TestSquareObjectWork.IsInvade(TestSquareObjectWork.Now(), new List<(DateTime startTime, DateTime endTime)>());
                if (invaded)
                {
                    squareObjectIo.UpdateInvasionResult(TestSquareObjectWork);
                }
            }
            return so;
        }

        public int New()
        {
            var next = 1;
            if (objects.Count > 0)
                next += objects.Max(obj => obj.Key);
            var vo = new SquareObjectVO
            {
                Id = next,
                CurrentState = new ProtoSquareObjectState
                {
                    InvasionHistory = new List<ProtoSquareObjectInvasionHistory>()
                    {
                        /*
                        new ProtoSquareObjectInvasionHistory
                        {
                        MonsterId = 1111,
                        MonsterAtk = 100,
                        MonsterCount = 10,
                        GettingAgencyExp = 222,
                        GettingCoreExp = 333,
                        InvasionLevel = 1,
                        InvasionTime= DateTime.UtcNow,
                        Power = 100,
                        PreviousShield = 20000,
                        RemainedShield = 19000
            
                        }
                        //*/
                    }
                },
                SquareObjectLevel = 1,
                CoreLevel = 1,
                AgencyLevel = 1,


            };

            objects.TryAdd(next, vo); ;
            return next;
        }

        internal SquareObjectVO Levelup(int id, int type, bool force)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                if (type == 0 && squareObject.SquareObjectLevel < 9)
                {
                    if (force)
                    {
                        squareObject.SquareObjectLevel++;
                    }
                    else
                    {
                        var needexp = AkaData.Data.GetSquareObject(squareObject.SquareObjectLevel).NeedExpForNextLevelUp;
                        if (squareObject.SquareObjectExp >= needexp)
                        {
                            squareObject.SquareObjectExp -= needexp;
                            squareObject.SquareObjectLevel++;
                        }
                    }
                }
                else if (type == 1 && squareObject.CoreLevel < 30)
                {
                    if (force)
                    {
                        squareObject.CoreLevel++;
                    }
                    else
                    {
                        var needexp = AkaData.Data.GetSquareObjectPlanetCore(squareObject.CoreLevel).NeedExpForNextLevelUp;
                        if (squareObject.CoreExp >= needexp)
                        {
                            squareObject.CoreExp -= needexp;
                            squareObject.CoreLevel++;
                        }
                    }
                }
                else if(type == 2 && squareObject.AgencyLevel < 30)
                {
                    if (force)
                    {
                        squareObject.AgencyLevel++;
                    }
                    else
                    {
                        var needexp = AkaData.Data.GetSquareObjectPlanetAgency(squareObject.AgencyLevel).NeedExpForNextLevelUp;
                        if (squareObject.AgencyExp >= needexp)
                        {
                            squareObject.AgencyExp -= needexp;
                            squareObject.AgencyLevel++;
                        }
                    }
                }

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Update(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO Leveldown(int id, int type, bool force)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                if (type == 0 && squareObject.SquareObjectLevel > 1)
                {
                    if (force == false)
                    {
                        var needexp = AkaData.Data.GetSquareObject(squareObject.SquareObjectLevel - 1).NeedExpForNextLevelUp;

                        squareObject.SquareObjectExp += needexp;
                    }
                    squareObject.SquareObjectLevel--;
                }
                else if (type == 1 && squareObject.CoreLevel > 1)
                {
                    if (force == false)
                    {
                        var needexp = AkaData.Data.GetSquareObjectPlanetCore(squareObject.CoreLevel - 1).NeedExpForNextLevelUp;

                        squareObject.CoreExp += needexp;
                    }
                    squareObject.CoreLevel--;
                }
                else if (type == 2 && squareObject.AgencyLevel > 1)
                {
                    if (force == false)
                    {
                        var needexp = AkaData.Data.GetSquareObjectPlanetAgency(squareObject.AgencyLevel - 1).NeedExpForNextLevelUp;

                        squareObject.AgencyExp += needexp;
                    }
                    squareObject.AgencyLevel--;
                }

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Update(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO EnergyInjection(int id, int power, int box)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var hasSeasonPass = false;
             //   var hasSeasonPass = await HasPremiumPass(powerInjection.UserId, db, accountDb);

                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;
                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);

                if (false == TestSquareObjectWork.UseEnergy(power, box, hasSeasonPass))
                    return null;

                if (TestSquareObjectWork.IsMaxPlanetBoxExp())
                {
                    squareObjectIo.Stop(TestSquareObjectWork);
                    //       res.RewardList = await squareObjectIo.GetRewards(TestSquareObjectWork.ObjectInfo);
                    //       return true;
                }
                else
                {
                    squareObjectIo.UpdateEnergy(TestSquareObjectWork);
                }
            }
            return so;
        }


        internal SquareObjectVO UpdateObject(SquareObjectVO vo)
        {
            var so = this.Get(vo.Id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(vo);
                var squareObject = vo;

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Update(TestSquareObjectWork);
                this.Set(vo);
            }
            return vo;
        }

        internal SquareObjectVO AddTimeSpan(int id, int time)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                squareObject.AddTimeSpan += time;

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Update(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO SetPremium(int id, bool isPremium)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                squareObject.IsPremium = isPremium;

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Update(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO Stop(int id)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;
                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);

                TestSquareObjectWork.Stop();

                squareObjectIo.Stop(TestSquareObjectWork);

                {
             //       res.RewardList = await squareObjectIo.GetRewards(TestSquareObjectWork.ObjectInfo);
             //       return true;
                }
            }
            return so;

        }


        internal SquareObjectVO GetReward(int id)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;
                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);

                var planetBox = Data.GetSquareObjectPlanetBox(squareObject.CurrentState.SquareObjectLevel, squareObject.CurrentState.CurrentPlanetBoxLevel);

                so.CurrentState.EnableReward = false;
                squareObject.SquareObjectExp += planetBox.GiveToLoseSquareObjectExp; 

                squareObjectIo.GetReward(TestSquareObjectWork);

                {
                    //       res.RewardList = await squareObjectIo.GetRewards(TestSquareObjectWork.ObjectInfo);
                    //       return true;
                }
            }
            return so;
        }

        internal SquareObjectVO Donation(int id)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Donation(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO HelpMe(int id)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;
                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                squareObjectIo.Help(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO Restart(int id)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                squareObject.UsedTicket += 1;
                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                TestSquareObjectWork.Recovery();
                squareObjectIo.Start(TestSquareObjectWork);
            }
            return so;
        }

        internal SquareObjectVO Start(int id, int level)
        {
            var so = this.Get(id);
            if (so != null)
            {
                var squareObjectIo = new SquareObjectIO(so);
                var squareObject = so;

                squareObject.UsedTicket += 1;
                var TestSquareObjectWork = new TestSquareObjectWork(squareObject);
                TestSquareObjectWork.Reset((uint)id, (uint)level);

                squareObjectIo.Start(TestSquareObjectWork);
            }
            return so;
        }

        public void Delete(int id)
        {
            objects.Remove(id);
        }

        internal void Delete(List<int> squareobjectList)
        {
            squareobjectList.ForEach(id => objects.Remove(id));
        }


        public class TestSquareObjectWork : SquareObjectWork
        {
            private SquareObjectVO so;

            public TestSquareObjectWork(SquareObjectVO so) : base(so) { this.so = so; }

            public override DateTime Now() => base.Now().AddMinutes(this.so.AddTimeSpan);


            public bool IsPremium() => this.so.IsPremium;
        }
    }
}
