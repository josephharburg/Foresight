using System;
using System.Collections.Generic;

namespace Foresight.Models
{
    public partial class UserData
    {
        public int DataId { get; set; }
        public int? FormId { get; set; }
        public int? Temperature { get; set; }
        public int? Aqi { get; set; }
        public int? Sleep { get; set; }
        public int? Stress { get; set; }
        public bool? IsSick { get; set; }
        public bool? WashingHands { get; set; }
        public bool? TouchingFace { get; set; }
        public bool? KeepingDistance { get; set; }
        public bool? IsIncliment { get; set; }
        public int? TotalPercentange { get; set; }
        public int? TempRiskPercent { get; set; }
        public int? AqiriskPercent { get; set; }
        public int? HandsRiskPercent { get; set; }
        public int? FaceRiskPercent { get; set; }
        public int? DistanceRiskPercent { get; set; }
        public DateTime? UserDataDateTime { get; set; }

        public virtual Form Form { get; set; }
    }
}
