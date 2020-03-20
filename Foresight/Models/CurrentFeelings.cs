using System;
using System.Collections.Generic;

namespace Foresight.Models
{
    public partial class CurrentFeelings
    {
        public int CurrentFeelingsId { get; set; }
        public int? FormId { get; set; }
        public bool? IsSick { get; set; }
        public int? SleepHours { get; set; }
        public int? Stress { get; set; }
        public bool? WashingHands { get; set; }
        public bool? Distance { get; set; }
        public int? SickRisk { get; set; }
        public int? StressRisk { get; set; }
        public int? SleepRisk { get; set; }
        public int? HandsRisk { get; set; }
        public int? DistanceRisk { get; set; }
        public DateTime? CurrentFeelingsDateTime { get; set; }
        
        public virtual Form Form { get; set; }
        public void GetRiskPercents()
        {
            if (IsSick == true)
            {
                this.SickRisk = 100;
            }
            else
            {
                this.SickRisk = 0;
            }
            
            if (SleepHours <= 5)
            {
                this.SleepRisk = 20;
            }
            else
            {
                this.StressRisk = 0;
            }
            if (Distance == true)
            {
                this.DistanceRisk = 10;
            }
            else
            {
                this.DistanceRisk = 0;
            }
            
            if (Stress >= 9)
            {
                this.StressRisk = 5;
            }
            else
            {
                this.StressRisk = 0;
            }
            if (WashingHands == true)
            {
                this.HandsRisk = -25;
            }
            else
            {
                this.HandsRisk = 20;
            }

        }
    }
}
