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
        public async Task<IActionResult> Index()
        {
            ForesightContext db = new ForesightContext();
            APIController apis = new APIController(_config);
            ViewInfo viewInfo = new ViewInfo();
            Weather w = await apis.GetWeather();
            var currentuser = db.Form.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            Dictionary<string, int> values = GetIntsFromForm(currentuser);
            viewInfo.TempOutput = w.TempOutputGenerator(values["cold"], values["hot"]);
            viewInfo.AqiOutput = w.AqiOutputGenerator(values["pollution"], values["allergies"]);
            w.IsRainingOrSnowing(values["incliment"]);
            viewInfo.Incliment = w.rainSnowStormOutput;
            viewInfo.news = await apis.GetNews();
            double count = 0;
            foreach (var item in viewInfo.news)
            {
                count += item.NewsRiskCalculator(values["disease"], values["sick"]);
            }
            count += w.AqiRiskCalculator(values["pollution"], values["allergies"]);
            count += w.TempRiskCalculator(values["cold"], values["hot"]);
            count += w.InclimentRiskCalculator(values["incliment"]);
            viewInfo.percentage = Math.Round((count / 70 * 100));
            if (viewInfo.percentage < 50)
            {
                viewInfo.colorOfBar = "bg-success";
                viewInfo.colorOfAnxiety = "foresight.png";
            }
            else if (viewInfo.percentage >= 50 && viewInfo.percentage < 70)
            {
                viewInfo.colorOfBar = "bg-warning";
                viewInfo.colorOfAnxiety = "concerned.png";
            }
            else if (viewInfo.percentage >= 70)
            {
                viewInfo.colorOfBar = "bg-danger";
                viewInfo.colorOfAnxiety = "NegOutcome.png";
            }
            return View(viewInfo);
        }
        public Dictionary<string, int> GetIntsFromForm(Form form)
        {
            
            string it = "1";
            Dictionary<string, int> values = new Dictionary<string, int>();
            values.Add("incliment", int.Parse(it));
            values.Add("cold", int.Parse(form.Question2));
            values.Add("hot", int.Parse(form.Question3));
            values.Add("pollution", int.Parse(form.Question4));
            values.Add("allergies", int.Parse(form.Question5));
            values.Add("disease", int.Parse(form.Question6));
            values.Add("crowds", int.Parse(form.Question7));
            values.Add("sick", int.Parse(form.Question8));
            return values;

        }


        public IActionResult EnterFormData(Form form)
        {
            ForesightContext db = new ForesightContext();
            Form f = new Form();
            f.UserName = User.Identity.Name;

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
            //{

            //    Question1 = form.Question1,
            //    Question2 = form.Question2,
            //    Question3 = form.Question3,
            //    Question4 = form.Question4,
            //    Question5 = form.Question5,
            //    Question6 = form.Question6,
            //    Question7 = form.Question7,
            //    Question8 = form.Question8,
            //    Question9 = form.Question9



            //});
            db.SaveChanges();

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
