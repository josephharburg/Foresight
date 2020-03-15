using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Foresight.Models;

namespace Foresight.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            return View();
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
