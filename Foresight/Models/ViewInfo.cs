using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foresight.Models
{
    public class ViewInfo
    {
        public double averagePercentage { get; set; }
        public string TempOutput { get; set; }
        public string AqiOutput { get; set; }
        public string Incliment { get; set; }
        public bool riskThreshold { get; set; }
        public double totalPercentage { get; set; }
        public string colorOfBar { get; set; }
        public string colorOfAnxiety { get; set; }

        public List<News> news = new List<News>();
        public List<UserData> userData = new List<UserData>();
        public List<UserData> otherUsers = new List<UserData>();
        public string dayOfWeek { get; set; }
        public double tempRiskPercent { get; set; }
        public double aqiRiskPercent { get; set; }
        public double inclimentRiskPercent { get; set; }
        public double newsRiskPercent { get; set; }
        public bool sickPercent { get; set; }
        public bool sleepRisk { get; set; }
        public bool distanceRisk { get; set; }
        public bool handsRisk { get; set; }
        public bool stressRisk { get; set; }
        public bool isSick { get; set; }
        public bool lookingUp { get; set; }
        public int allUsersAverage { get; set; }
        public void GetRiskPercents(Weather w, Dictionary<string, int> values, CurrentFeelings currentFeelings)
        {
            
            double count = 0;
            
             foreach (var item in this.news)
            {
                count += item.NewsRiskCalculator(values["disease"], values["sick"]);
            }
            this.newsRiskPercent = (count / 110) * 100;
          
            double aqiriskcount = w.AqiRiskCalculator(values["pollution"], values["allergies"]);
            this.aqiRiskPercent = (aqiriskcount / 110) * 100;
          
            count += aqiriskcount;
            double tempriskcount = w.TempRiskCalculator(values["cold"], values["hot"]);
            this.tempRiskPercent = (tempriskcount / 110) * 100;
          
            count += tempriskcount;
            double inclimentriskcount = w.InclimentRiskCalculator(values["incliment"]);
            this.inclimentRiskPercent = (inclimentriskcount / 110) * 100;
           
            count += inclimentriskcount;

            count += (int)currentFeelings.DistanceRisk;
            if(currentFeelings.DistanceRisk > 0)
            {
                this.distanceRisk = true;
            }
            count += (int)currentFeelings.HandsRisk;
            if (currentFeelings.HandsRisk > 0)
            {
                this.handsRisk = true;
            }
            count += (int)currentFeelings.SleepRisk;
            if (currentFeelings.SleepRisk != 0)
            {
                this.sleepRisk = true;
            }
            count += (int)currentFeelings.StressRisk;
            if (currentFeelings.StressRisk != 0)
            {
                this.stressRisk = true;
            }

            
            if (currentFeelings.SickRisk == 100)
            {
                this.totalPercentage = 100;
                this.isSick = true;
            }
            else
            {
                this.totalPercentage = Math.Round((count / 110 * 100));
            }

            if(this.totalPercentage > 100)
            {
                this.totalPercentage = 100;
            }
        }
       
    }
}
