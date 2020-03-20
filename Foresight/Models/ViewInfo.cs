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

        public double totalPercentage { get; set; }
        public string colorOfBar { get; set; }
        public string colorOfAnxiety { get; set; }

        public List<News> news = new List<News>();
        public List<UserData> userData = new List<UserData>();
        public string dayOfWeek { get; set; }
        public double tempRiskPercent { get; set; }
        public double aqiRiskPercent { get; set; }
        public double inclimentRiskPercent { get; set; }
        public double newsRiskPercent { get; set; }
        
        public double GetRiskPercentsFromAPI(Weather w, Dictionary<string, int> values)
        {
            
            double count = 0;
            
             foreach (var item in this.news)
            {
                count += item.NewsRiskCalculator(values["disease"], values["sick"]);
            }
            this.newsRiskPercent = (count / 130) * 100;
          
            double aqiriskcount = w.AqiRiskCalculator(values["pollution"], values["allergies"]);
            this.aqiRiskPercent = (aqiriskcount / 130) * 100;
          
            count += aqiriskcount;
            double tempriskcount = w.TempRiskCalculator(values["cold"], values["hot"]);
            this.tempRiskPercent = (tempriskcount / 130) * 100;
          
            count += tempriskcount;
            double inclimentriskcount = w.InclimentRiskCalculator(values["incliment"]);
            this.inclimentRiskPercent = (inclimentriskcount / 130) * 100;
           
            count += inclimentriskcount;
          
           
            return count;
        }
    }
}
