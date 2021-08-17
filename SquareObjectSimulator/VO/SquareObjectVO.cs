using CommonProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SquareObjectSimulator.VO
{
    public class SquareObjectVO : ProtoSquareObject
    {
        public int Id { get; set; }

        public int Donate { get; set; }

        public int Help { get; set; }

        public int TotalDonate { get; set; }

        public int TotalHelp { get; set; }

        public bool IsPremium { get; set; }

        public int AddTimeSpan { get; set; }

        public DateTime DonationTime { get; set; }
        public DateTime HelpTime { get; set; }

        public int UsedTicket { get; set; }

        public int EnergyCycle { get; set; }
        public double EnergyQuantity { get; set; }
        public int PowerCycle { get; set; }
        public double PowerQuantity { get; set; }

        public List<ProtoSquareObjectInvasionHistory> InvasionHistory { get; set; }
    }
}
