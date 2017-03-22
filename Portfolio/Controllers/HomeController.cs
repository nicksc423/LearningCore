using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Portfolio.Extensions;

namespace Portfolio.Controllers
{
    public class HomeController : Controller
    {
        private const string viewCountKey = "viewCount";
        private const string tomogachiFullnessKey = "Fullness";
        private const string tomogachiHappinessKey = "Happiness";
        private const string tomogachiMealsKey = "Meals";
        private const string tomogachiEnergyKey = "Energy";
        private const string tomogachiUpdateKey = "Update";
        private const string tomogachiSetKey = "Set";

        private static Random rand = new Random();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("survey")]
        public IActionResult Survey(string Name, string Location, string Language, string Comment)
        {
            ViewBag.Name = Name;
            ViewBag.Location = Location;
            ViewBag.Language = Language;
            ViewBag.Comment = Comment;
            return View("Result");
        }

        [HttpGet]
        [Route("passcode")]
        public IActionResult landingPage()
        {
            return View("Passcode");
        }

        [HttpGet]
        [Route("generate")]
        public JsonResult generate()
        {
            int viewCount = HttpContext.Session.GetInt32(viewCountKey) + 1 ?? default(int) + 1;
            HttpContext.Session.SetInt32(viewCountKey, viewCount);
            var obj = new { code = RandomString(14), viewCount = viewCount };
            return Json(obj);
        }

        [HttpGet]
        [Route("tomogachi")]
        public IActionResult tomogachi()
        {
            if (!HttpContext.Session.GetBoolean(tomogachiSetKey) ?? true)
            {
                resetTomogachi();
            }
            
            return View("Tomogachi");
        }

        [HttpGet]
        [Route("tomogachiStatus")]
        public JsonResult tomogachiStatus()
        {
            var tomogachi = new
            {
                Fullness = HttpContext.Session.GetInt32(tomogachiFullnessKey) ?? default(int),
                Happiness = HttpContext.Session.GetInt32(tomogachiHappinessKey) ?? default(int),
                Meals = HttpContext.Session.GetInt32(tomogachiMealsKey) ?? default(int),
                Energy = HttpContext.Session.GetInt32(tomogachiEnergyKey) ?? default(int),
                Update = HttpContext.Session.GetString(tomogachiUpdateKey)
            };

            return Json(tomogachi);
        }

        private void resetTomogachi()
        {
            HttpContext.Session.SetInt32(tomogachiFullnessKey, 20);
            HttpContext.Session.SetInt32(tomogachiHappinessKey, 20);
            HttpContext.Session.SetInt32(tomogachiMealsKey, 50);
            HttpContext.Session.SetInt32(tomogachiEnergyKey, 10);
            HttpContext.Session.SetString(tomogachiUpdateKey, "");
        }

        [HttpPost]
        [Route("tomogachiFeed")]
        public IActionResult tomogachiFeed()
        {
            HttpContext.Session.SetInt32(tomogachiFullnessKey, (HttpContext.Session.GetInt32(tomogachiFullnessKey) ?? default(int)) + 5);
            return RedirectToAction("tomogachiStatus");
        }

        [HttpPost]
        [Route("tomogachiPlay")]
        public IActionResult tomogachiPlay()
        {
            //Playing costs 5 energy and gains a random amount of happiness between 5 and 10 
            int currentEnergy = HttpContext.Session.GetInt32(tomogachiEnergyKey) ?? default(int);
            if (currentEnergy < 5)
            {
                HttpContext.Session.SetString(tomogachiUpdateKey, "Your Tomogachi is to tired to play!");
            } else {
                int generatedHappiness = rand.Next(5, 10);
                int currentHappiness = HttpContext.Session.GetInt32(tomogachiHappinessKey) ?? default(int);
                HttpContext.Session.SetInt32(tomogachiEnergyKey, currentEnergy -= 5);
                HttpContext.Session.SetInt32(tomogachiHappinessKey, currentHappiness + generatedHappiness);
                HttpContext.Session.SetString(tomogachiUpdateKey, string.Format("You played with your tomogachi and generated {0} Happiness", generatedHappiness));
            }
            return RedirectToAction("tomogachiStatus");
        }

        [HttpPost]
        [Route("tomogachiWork")]
        public IActionResult tomogachiWork()
        {
            return RedirectToAction("tomogachiStatus");
        }

        [HttpPost]
        [Route("tomogachiSleep")]
        public IActionResult tomogachiSleep()
        {
            return RedirectToAction("tomogachiStatus");
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}
