using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foresight.Models
{
    public class ViewInfo
    {
        public string TempOutput { get; set; }
        public string AqiOutput { get; set; }
        public string Incliment { get; set; }

        public double percentage { get; set; }
        public string colorOfBar { get; set; }
        public string colorOfAnxiety { get; set; }
        public List<News> news = new List<News>();
        public string latitude { get; set; }
        public string longitude { get; set; }

        public double tempriskpercent { get; set; }
        public double aqiriskpercent { get; set; }
        public double inclimentriskpercent { get; set; }
        public double newsrisk { get; set; }
    }
}
