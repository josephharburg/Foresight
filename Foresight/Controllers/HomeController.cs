using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Foresight.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;

namespace Foresight.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _config;
        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }
        [Authorize]
        public IActionResult Index()
        {
            ForesightContext db = new ForesightContext();
            List<UserData> userData = new List<UserData>();

            ViewInfo view = new ViewInfo();
            var DayofLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6);
            var cur = db.Form.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            HttpContext.Session.SetInt32("current", cur.FormId);
            int highest = 0;
            foreach (var item in db.UserData)
            {
                if(item.UserDataDateTime >= DayofLastWeek && item.FormId == HttpContext.Session.GetInt32("current"))
                {
                    userData.Add(item);
                }
                
            }
            
            view.userData = userData;
           
            return View(view);
        }
        public Dictionary<string, int> GetIntsFromForm(Form form)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();
            values.Add("incliment", int.Parse(form.Question1));
            values.Add("cold", int.Parse(form.Question2));
            values.Add("hot", int.Parse(form.Question3));
            values.Add("pollution", int.Parse(form.Question4));
            values.Add("allergies", int.Parse(form.Question5));
            values.Add("disease", int.Parse(form.Question6));
            values.Add("crowds", int.Parse(form.Question7));
            values.Add("sick", int.Parse(form.Question8));
            return values;

        }

        //public IActionResult SearchView(string choice)
        //{
        //    foreach(var item in db)
        //    {
        //        if(item.choice == )
        //    }

        //}
        [Authorize]
        public async Task<IActionResult> GetPrediction()
        {
            ForesightContext db = new ForesightContext();
            APIController apis = new APIController(_config);
            ViewInfo viewInfo = new ViewInfo();
            
            Weather w = await apis.GetWeather();
            
            //Get MOST RECENT CHANGES TO FORM!!!
            var currentuser = db.Form.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            CurrentFeelings currentFeelings = db.CurrentFeelings.Where(x => x.FormId == currentuser.FormId).FirstOrDefault();
            Dictionary<string, int> values = GetIntsFromForm(currentuser);
            viewInfo.TempOutput = w.TempOutputGenerator(values["cold"], values["hot"]);
            viewInfo.AqiOutput = w.AqiOutputGenerator(values["pollution"], values["allergies"]);
            w.IsRainingOrSnowing(values["incliment"]);
            viewInfo.Incliment = w.rainSnowStormOutput;
            viewInfo.news = await apis.GetNews();
            double count = 0;
            double percent = viewInfo.GetRiskPercentsFromAPI(w, values);
            count += percent;
            currentFeelings.GetRiskPercents();
            List<double> highestRisk = new List<double>()
            {
                viewInfo.newsRiskPercent,
                viewInfo.tempRiskPercent,
                viewInfo.inclimentRiskPercent,
                viewInfo.aqiRiskPercent,
                (double)currentFeelings.DistanceRisk,
                (double)currentFeelings.HandsRisk,
                (double)currentFeelings.SleepRisk,
                (double)currentFeelings.StressRisk
            };
            double highest = 0;
            
            foreach(var item in highestRisk)
            {
                if(item > highest)
                {
                    highest = item;
                }
            }
            count += (int)currentFeelings.DistanceRisk;
            count += (int)currentFeelings.HandsRisk;
            count += (int)currentFeelings.SleepRisk;
            count += (int)currentFeelings.StressRisk;
            viewInfo.totalPercentage = Math.Round((count / 130 * 100));
            if(currentFeelings.SickRisk == 100)
            {
                viewInfo.totalPercentage = 100;
            }

            UserData userData = new UserData()
            {
                TotalPercentange = (int)viewInfo.totalPercentage,
                UserDataDateTime = DateTime.Now,
                Temperature = (int)(w.temperature * 1.8) + 32,
                TempRiskPercent = (int)viewInfo.tempRiskPercent,
                Aqi = (int)w.aqi,
                AqiriskPercent = (int)viewInfo.aqiRiskPercent,
                FormId = currentuser.FormId,
                IsIncliment = w.rainSnowStorm,
                Sleep = (int)currentFeelings.SleepHours,
                KeepingDistance = currentFeelings.Distance,
                WashingHands = currentFeelings.WashingHands,
                IsSick = currentFeelings.IsSick,
                Stress = (int)currentFeelings.Stress
            };
            db.UserData.Add(userData);
            db.SaveChanges();
            if(currentFeelings.SickRisk == 100)
            {
                viewInfo.colorOfBar = "bg-danger";
                viewInfo.colorOfAnxiety = "NegOutcome.png";
            }
            else if (viewInfo.totalPercentage < 50)
            {
                viewInfo.colorOfBar = "bg-success";
                viewInfo.colorOfAnxiety = "foresight.png";
            }
            else if (viewInfo.totalPercentage >= 50 && viewInfo.totalPercentage < 70)
            {
                viewInfo.colorOfBar = "bg-warning";
                viewInfo.colorOfAnxiety = "concerned.png";
            }
            else if (viewInfo.totalPercentage >= 70)
            {
                viewInfo.colorOfBar = "bg-danger";
                viewInfo.colorOfAnxiety = "NegOutcome.png";
            }
            
            return View(viewInfo);
        }

        
        public IActionResult EnterFormData(Form form)
        {
            ForesightContext db = new ForesightContext();
            Form f = new Form();
            f.UserName = User.Identity.Name;
            f.FormDate = DateTime.Now;
            f.Question1 = form.Question1;
            f.Question2 = form.Question2;
            f.Question3 = form.Question3;
            f.Question4 = form.Question4;
            f.Question5 = form.Question5;
            f.Question6 = form.Question6;
            f.Question7 = form.Question7;
            f.Question8 = form.Question8;
            f.Question9 = form.Question9;
            db.Add(f);
           
            db.SaveChanges();

            return RedirectToAction("Index");
        }
       
        
        public IActionResult AddCurrentFeelings(CurrentFeelings c)
        {
            ForesightContext db = new ForesightContext();
            var user = db.Form.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            c.FormId = user.FormId;
            c.CurrentFeelingsDateTime = DateTime.Now;
            db.Add(c);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult CurrentFeelings()
        {
            return View();
        }
        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
