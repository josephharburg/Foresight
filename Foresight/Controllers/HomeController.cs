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
            Dashboard view = new Dashboard();

            var DayofLastWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 6);

            //Gets current user data and assigns it to a session variable
            var cur = db.Form.Where(x => x.UserName == User.Identity.Name).OrderByDescending(x => x.FormDate).FirstOrDefault();
            HttpContext.Session.SetInt32("current", cur.FormId);
            foreach (var item in db.UserData)
            {
                if (item.UserDataDateTime >= DayofLastWeek && item.FormId == HttpContext.Session.GetInt32("current"))
                {
                    view.userData.Add(item);
                }
                else if (item.UserDataDateTime >= DayofLastWeek && item.FormId != HttpContext.Session.GetInt32("current"))
                {
                    view.otherUsers.Add(item);
                }
            }

            view.GetAverageRiskPercents();

            view.AreThingsLookingUp();
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
            //values.Add("threshold", int.Parse(form.Question9));
            return values;

        }

        public IActionResult SearchView(string choice)
        {
            ForesightContext db = new ForesightContext();
            ViewInfo viewInfo = new ViewInfo();
            var user = db.UserData.Where(x => x.FormId == HttpContext.Session.GetInt32("current"));
            if (choice == "Highest Of All Time")
            {
                var highestalltime = user.OrderByDescending(x => x.TotalPercentange);
                var list = highestalltime.ToArray();
                for (int x = 0; x <= 7; x++)
                {
                    viewInfo.userData.Add(list[x]);
                }
            }
            else if (choice == "Past Month")
            {
                var LastMonth = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek - 30);
                foreach (var item in db.UserData)
                {
                    if (item.UserDataDateTime >= LastMonth && item.FormId == HttpContext.Session.GetInt32("current"))
                    {
                        viewInfo.userData.Add(item);
                    }
                }
            }

            return View(viewInfo);


        }
        public IActionResult ShowPastPrediction(int user)
        {
            ForesightContext db = new ForesightContext();
            UserData d = db.UserData.Where(x => x.DataId == user).FirstOrDefault();
            return View(d);
        }
        [Authorize]
        public async Task<IActionResult> GetPrediction()
        {
            ForesightContext db = new ForesightContext();
            APIController apis = new APIController(_config);
            ViewInfo viewInfo = new ViewInfo();

            //Gets the current user based on latest update to profile by ordering dates by descending and selecting first one
            var currentuser = db.Form.Where(x => x.UserName == User.Identity.Name).OrderByDescending(x => x.FormDate).FirstOrDefault();

            //Same idea except for CurrentFeelings table
            CurrentFeelings currentFeelings = db.CurrentFeelings.OrderByDescending(x => x.CurrentFeelingsDateTime).FirstOrDefault();

            //Gets all the weather infomation and puts into a model
            Weather w = await apis.GetWeather(currentFeelings.City);

            //Puts labels to integers in Form from current user
            Dictionary<string, int> values = GetIntsFromForm(currentuser);

            //Creates the text output based on user profile and api data for temperature then air quality
            viewInfo.TempOutput = w.TempOutputGenerator(values["cold"], values["hot"]);
            viewInfo.AqiOutput = w.AqiOutputGenerator(values["pollution"], values["allergies"]);

            //Checks to see if its raining and if the user is afraid of incliment weather and creates output for either case.
            w.IsRainingOrSnowing(values["incliment"]);
            viewInfo.Incliment = w.rainSnowStormOutput;

            //Gets the news
            viewInfo.news = await apis.GetNews();

            //Uses all the data collected from current feelings to get the risk percentages based on user values
            currentFeelings.GetRiskPercents();

            //Uses all the data collected to get the risk percentages based on user values and the api results
            viewInfo.GetRiskPercents(w, values, currentFeelings);
           
            //Accumulates the Data and adds it to the database for further use on Dashboard and Search
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
                Stress = (int)currentFeelings.Stress,
                City = currentFeelings.City,
                Email = User.Identity.Name
            };
            db.UserData.Add(userData);
            db.SaveChanges();

            //This Adjusts the view to show proper risk colors and percentage
            #region colorsofrisk
            if (currentFeelings.SickRisk == 100)
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
            
            #endregion
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

            return RedirectToAction("CurrentFeelings");
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
        [Authorize]
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
