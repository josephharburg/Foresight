using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foresight.Models
{
    public class News
    {
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string urlToImage { get; set; }
        public string content { get; set; }

        public double NewsRiskCalculator(int disease, int sick)
        {
            if (disease > 5 && this.content.ToLower().Contains("coronavirus"))
            {
                return disease;
            }
            else if (sick > 5 && this.content.ToLower().Contains("coronavirus"))
            {
                return sick;
            }
            else if(disease > 5 && sick > 5 && this.content.ToLower().Contains("coronavirius")){
                return sick + disease;
            }
            else if(this.title.ToLower().Contains("coronavirus"))
            {
                return 10;
            }
            else
            {
                return 0;
            }
        }
    }
}
