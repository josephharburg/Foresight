using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foresight.Models
{
    public class Weather
    {
        public double temperature { get; set; }
        public double humidity { get; set; }
        public double windspeed { get; set; }

        public string weather { get; set; }
        public bool rainSnowStorm { get; set; }
        public string rainSnowStormOutput { get; set; }
        public double aqi { get; set; }
        public bool humid { get; set; }
        public string coldHot { get; set; }
        public string pollution { get; set; }

        public void TempandHumidSet()
        {
            #region Temperature&Humidity
            if (this.temperature > 12 && this.humidity > 55)
            {
                this.humid = true;
            }
            else
            {
                this.humid = false;
            }
            if (this.temperature >= 12 && this.temperature <= 21)
            {
                this.coldHot = "Warm";
            }
            else if (this.temperature >= 21)
            {
                this.coldHot = "Hot";
            }
            else if (this.temperature < 12 && this.temperature > 0)
            {
                this.coldHot = "Cold";
            }
            else if (this.temperature <= 0)
            {
                this.coldHot = "Freezing";
            }
            #endregion

        }
        public string TempOutputGenerator(int cold, int hot)
        {
            double convertedTemp = (this.temperature * 1.8) + 32;
            string output;
            TempandHumidSet();
            if (this.humid)
            {
                output = $"It's {this.coldHot} outside. The temperature is {convertedTemp} degrees and it's humid out as well. ";
            }
            else
            {
                output = $"It's {this.coldHot} outside. The temperature is {convertedTemp} degrees. ";
            }

            #region outputforTemp
            if (this.coldHot == "Cold" || this.coldHot == "Freezing" && cold > 5)
            {
                output += "You told me you have an aversion to cold weather. So factor that in before going out!";

            }
            else if (this.coldHot == "Cold" || this.coldHot == "Freezing" && cold < 5)
            {
                output += "You dont have a problem with cold but be careful you could get frostbite or freeze!";
            }

            if (this.coldHot == "Hot" && hot > 5)
            {
                output += "You told me you have an aversion to hot weather. So factor that in before going out!";

            }
            else if (this.coldHot == "Hot" && hot < 5)
            {
                output += "You don't have a problem with heat, but be cafeful you could get dehydrated or get sunburned!";
            }

            if (this.coldHot == "Warm")
            {
                output += "I dont see any problem going out because of temperature now, just be careful of other risk factors!";
            }
            #endregion

            return output;
        }
        public double TempRiskCalculator(int cold, int hot)
        {
            TempandHumidSet();
            double count = 0;
            if (this.coldHot == "Cold" || this.coldHot == "Freezing" && cold > 5)
            {
                count += cold;
            }
            else if (this.coldHot == "Hot" && hot > 5)
            {
                count += hot;
            }
            return count;
        }
        public void IsRainingOrSnowing(int incliment)
        {
            if (this.weather == "09d" || this.weather == "10d" || this.weather == "10n" && incliment > 5)
            {
                this.rainSnowStormOutput = "Its raining out! You could be washed away in a flood!";
                this.rainSnowStorm = true;
            }
            else if (this.weather == "13d" && incliment > 5)
            {
                this.rainSnowStormOutput = "Its snowing out! You could get caught in a Blizzard!";
                this.rainSnowStorm = true;
            }
            else if (this.weather == "11d" && incliment > 5)
            {
                this.rainSnowStormOutput = "Theres a thunderstorm out there! You could get struck by Lighting!";
                this.rainSnowStorm = true;
            }
            else
            {
                this.rainSnowStormOutput = "Its not raining, snowing, or storming. This risk factor is not a concern at the moment, but that could change suddenly!";
                this.rainSnowStorm = false;
            }

        }
        public double InclimentRiskCalculator(int incliment)
        {
            double count = 0;
            if (this.rainSnowStorm && incliment > 5)
            {
                count = incliment;
            }
            return count;
        }

        public string AqiOutputGenerator(int pollution, int allergies)
        {

            #region Level Calculator
            if (this.aqi >= 0 && this.aqi < 50)
            {
                this.pollution = "Safe";
            }
            else if (this.aqi >= 50 && this.aqi < 100)
            {
                this.pollution = "Moderate";
            }
            else if (this.aqi >= 100 && this.aqi < 300)
            {
                this.pollution = "Unhealthy";
            }
            else if (this.aqi >= 300)
            {
                this.pollution = "Hazardous";
            }
            #endregion
            string output = $"The air pollutant is at {this.aqi}% and the air is at {this.pollution} levels.";
            if (this.pollution == "Safe")
            {
                output += "The air pollution is low and air quality is good but be careful of other risk factors!";
            }
            else if (this.pollution != "Safe" && this.pollution != "Hazardous" && allergies >= 5 && pollution < 5)
            {
                output += "The air pollution is higher than normal.There is a high chance of your allergies acting up be careful!";
            }
            else if (this.pollution != "Safe" && this.pollution != "Hazardous" && pollution >= 5 && allergies < 5)
            {
                output += "The air pollution is higher than normal. You have a fear of pollution, you might not be able to breathe if you leave your house!";
            }
            else if (this.pollution != "Safe" && this.pollution != "Hazardous" && pollution >= 5 && allergies >= 5)
            {
                output += "The air pollution is higher than normal. You have a fear of pollution and you have allergies you might not be able to breathe if you leave your house!";
            }
            else if (this.pollution != "Safe" && this.pollution != "Hazardous")
            {
                output += "The air pollution is higher than normal. Be aware of this before you go out.";
            }
            else
            {
                output += "The air pollution is terribly high! It's at hazardous Levels! DONT GO OUT!!!";
            }
            return output;
        }
        public double AqiRiskCalculator(int pollution, int allergies)
        {
            double risk = 0;
            if (this.aqi >= 50 && this.aqi < 300 && pollution >= 5 && allergies <= 5)
            {
                risk = pollution;
            }
            else if (this.aqi >= 50 && this.aqi < 300 && allergies >= 5 && pollution <= 5)
            {
                risk = allergies;
            }
            else if (this.aqi >= 50 && this.aqi < 300 && allergies >= 5 && pollution >= 5)
            {
                risk = allergies + pollution;
            }
            else if(this.aqi >= 50 && this.aqi < 300)
            {
                risk = 10;
            }
            else if (this.aqi >= 300)
            {
                risk = 80;
            }
            else
            {
                risk = 0;
            }

            return risk;
        }
    }
}
