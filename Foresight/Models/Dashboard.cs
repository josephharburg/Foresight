using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Foresight.Models
{
    public class Dashboard
    {
        
        public int averagePercentage {get; set;}
        public int allUsersAverage { get; set; }
        public bool lookingUp { get; set; }
        public List<UserData> userData = new List<UserData>();
        public List<UserData> otherUsers = new List<UserData>();
        public void GetAverageRiskPercents()
        {
            int average = 0;
            if (this.userData.Count > 1)
            {
                foreach (var item in this.userData)
                {
                    average += (int)item.TotalPercentange;
                }

                average = average / this.userData.Count;
                this.averagePercentage = average;
            }
            else
            {
                this.averagePercentage = 0;
            }
            average = 0;
            if(this.otherUsers.Count > 1)
            {
                foreach (var item in this.otherUsers)
                {
                    if (item.TotalPercentange != null)
                    {
                        average += (int)item.TotalPercentange;
                    }
                }
                average = average / this.otherUsers.Count;
                this.allUsersAverage = average;
            }
            else
            {
                this.allUsersAverage = 0;
            }
         

        }
        public void AreThingsLookingUp()
        {
            if (this.userData.Count > 2)
            {
                if (this.userData[this.userData.Count - 1].TotalPercentange > this.userData[this.userData.Count - 2].TotalPercentange)
                {
                    this.lookingUp = false;
                }
                else
                {
                    this.lookingUp = true;
                }
            }
        }
    }
}
