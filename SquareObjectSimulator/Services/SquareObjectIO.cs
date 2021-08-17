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




    public class SquareObjectIO
    {
        private SquareObjectVO so;

        public SquareObjectIO(SquareObjectVO so)
        {
            this.so = so;
        }

        internal void Start(SquareObjectWork squareObjectWork)
        {
            Update(squareObjectWork);
        }

        internal void Stop(SquareObjectWork squareObjectWork)
        {
            Update(squareObjectWork);
        }

        internal void UpdateEnergy(SquareObjectWork squareObjectWork)
        {
            Update(squareObjectWork);
        }

        internal void Donation(SquareObjectWork squareObjectWork)
        {
            var now  = squareObjectWork.Now();
            Update(squareObjectWork);

            var donate = this.so.Donate + 1;
            if (donate <= 10)
            {
                var gettingEnergy = (int)(float.Epsilon + Data.GetConstant(DataConstantType.SQUARE_OBJECT_GETTING_ENERGY_DONATION_VALUE)?.Value ?? 0);
                this.so.Donate = donate;
                this.so.CurrentState.CoreEnergy += gettingEnergy;
                this.so.TotalDonate++;
            }
        }

        internal void Help(SquareObjectWork squareObjectWork)
        {
            var now = squareObjectWork.Now();
            Update(squareObjectWork);

            var help = this.so.Help + 1;
            if (help <= 10)
            {
                var donatePower = (int)(float.Epsilon + Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_DONATION_VALUE)?.Value ?? 0);
                this.so.Help = help;
                this.so.CurrentState.SquareObjectPower += donatePower;
                this.so.TotalHelp++;
            }
        }


        public void UpdateInvasionResult(SquareObjectWork work)
        {
            Update(work);

            var coreExp = work.ObjectInfo.CoreExp + work.State.InvasionHistory.Where(history => history.GettingCoreExp !=0).Select(history => { var exp = history.GettingCoreExp; history.GettingCoreExp = 0; return exp; }).Sum(exp => exp);
            var agencyExp = work.ObjectInfo.AgencyExp + work.State.InvasionHistory.Where(history => history.GettingAgencyExp != 0).Select(history => { var exp = history.GettingAgencyExp; history.GettingAgencyExp = 0; return exp; }).Sum(exp => exp);
            so.CoreExp = (int)coreExp;
            so.AgencyExp = (int)agencyExp;

            so.InvasionHistory = new List<ProtoSquareObjectInvasionHistory>(so.CurrentState.InvasionHistory);
            so.InvasionHistory.Reverse();

        }

        internal void GetReward(SquareObjectWork work)
        {
            Update(work);

        }



        internal void Update(SquareObjectWork squareObjectWork)
        {
            var squareObject = squareObjectWork.ObjectInfo;

            Copy(so, squareObject);
            Copy(so.CurrentState, squareObject.CurrentState);


            var acquistionEnergyMin = Data.GetConstant(DataConstantType.SQUARE_OBJECT_PLANET_CORE_ENERGY_ACQUISITION_UNIT_MINUTE).Value;
            var quantityEnergy = Data.GetConstant(DataConstantType.SQUARE_OBJECT_PLANET_CORE_ENERGY_QUANTITY_EACH_GET).Value;

            var powerReduceMin = Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_REDUCE_UNIT_MINUTE).Value;
            var quantityPower = Data.GetConstant(DataConstantType.SQUARE_OBJECT_POWER_REDUCE_QUANTITY_EACH).Value;

            so.EnergyCycle = (int)(float.Epsilon + acquistionEnergyMin);
            so.PowerCycle = (int)(float.Epsilon + powerReduceMin);
            so.EnergyQuantity = quantityEnergy;
            so.PowerQuantity = quantityPower;

            var refreshBaseHour = (int)(Data.GetConstant(DataConstantType.START_DAY_BASE_HOUR).Value + float.Epsilon);
            var utcNow = squareObjectWork.Now().AddHours(-refreshBaseHour);
            var initDateTime = new DateTime(utcNow.Year, utcNow.Month, utcNow.Day, refreshBaseHour, 0, 0, DateTimeKind.Utc);

            if (this.so.DonationTime < initDateTime)
            {
                this.so.Donate = 0;
                this.so.DonationTime = squareObjectWork.Now();
            }
            if (this.so.HelpTime < initDateTime)
            {
                this.so.Help = 0;
                this.so.HelpTime = squareObjectWork.Now();
            }

        }



        private void Copy<TL,TR>(TL dest, TR source)
        {

            var fields = typeof(TR).GetFields();
            foreach (var field in fields)
            {
                try
                {
                    if (field.FieldType.IsValueType)
                    {
                        field.SetValue(dest, field.GetValue(source));
                    }
                    else
                    {
                        var obj = field.GetValue(source);
                        if (obj == null)
                        {
                            field.SetValue(dest, null);
                            continue;
                        }

                        if (obj is ICloneable)
                        {
                            var cloneable = obj as ICloneable;
                            field.SetValue(dest, cloneable.Clone());
                        }
                        else if (obj is System.Collections.IEnumerable)
                        {
                            field.SetValue(dest, Activator.CreateInstance(obj.GetType(), obj));
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

            var properties = typeof(TR).GetProperties().Where(property => property.CanRead && property.CanWrite);
            foreach (var property in properties)
            {
                try
                {
                    if (property.PropertyType.IsValueType)
                    {
                        property.SetValue(dest, property.GetValue(source));
                    }
                    else
                    {
                        var obj = property.GetValue(source);
                        if (obj == null)
                        {
                            property.SetValue(dest, null);
                            continue;
                        }

                        if (obj is ICloneable)
                        {
                            var cloneable = obj as ICloneable;
                            property.SetValue(dest, cloneable.Clone());
                        }
                        else if (obj is System.Collections.IEnumerable)
                        {
                            property.SetValue(dest, Activator.CreateInstance(obj.GetType(), obj));
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

        }

    }
}
