using System;
using System.Collections.Generic;

namespace Foresight.Models
{
    public partial class Form
    {
        public Form()
        {
            CurrentFeelings = new HashSet<CurrentFeelings>();
            UserData = new HashSet<UserData>();
        }

        public int FormId { get; set; }
        public string UserName { get; set; }
        public string Question1 { get; set; }
        public string Question2 { get; set; }
        public string Question3 { get; set; }
        public string Question4 { get; set; }
        public string Question5 { get; set; }
        public string Question6 { get; set; }
        public string Question7 { get; set; }
        public string Question8 { get; set; }
        public string Question9 { get; set; }
        public DateTime? FormDate { get; set; }

        public virtual ICollection<CurrentFeelings> CurrentFeelings { get; set; }
        public virtual ICollection<UserData> UserData { get; set; }
    }
}
