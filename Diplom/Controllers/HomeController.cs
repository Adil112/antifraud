using Diplom.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Diplom.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            antifraudContext db = new antifraudContext();
            var users = db.Users.ToList();
            return View(users);
        }
        [HttpPost]
        public IActionResult Index(string SearchString)
        {
            string search = "";
            if (SearchString != null) search = SearchString;
            antifraudContext db = new antifraudContext();
            var users = db.Users.ToList();
            if (!String.IsNullOrEmpty(search))
            {
                users = db.Users.Where(s => s.Fio.Contains(search)).ToList();
            }
            return View(users);
        }

        public partial class FormModel
        {
            public int Time { get; set; }
             public string Name { get; set; }
        }

        public partial class SecModel
        {
            public int Time { get; set; }
            public string Name { get; set; }
        }
        public IActionResult Info(Guid id)
        {
            antifraudContext db = new antifraudContext();
            var user = db.Users.Where(s=>s.UserId == id).FirstOrDefault();
            ViewBag.UserId = user.UserId;
            ViewBag.UserFio = user.Fio;
            ViewBag.UserMark = user.Mark;
            string flag = "";
            if (user.Mark <= 30) flag = "Зеленый";
            if (user.Mark <= 75) flag = "Желтый";
            else  flag = "Красный";
            ViewBag.Flag = flag;

            var nowT = DateTime.Now;

            var lastSession = db.Sessions.Where(r=>r.Users == id).Where(s => s.FinishTime < nowT).First();
            ViewBag.LastStart = lastSession.StartTime;
            ViewBag.LastFinish = lastSession.FinishTime;
            ViewBag.LastDur = lastSession.FinishTime - lastSession.StartTime;
            ViewBag.LastCountry = db.Countries.Where(s => s.CountryId == lastSession.Country).FirstOrDefault().Name;
            ViewBag.LastPK = "Нет";
            if (lastSession.Pk) ViewBag.LastPK = "Да"; 

           List <FormModel> forms = new List<FormModel>();
            List<SecModel> secs = new List<SecModel>();
                List<SecModel> secs2 = new List<SecModel>();

            Random rnd = new Random();
            int formCount = rnd.Next(1,2);
            int secCount = rnd.Next(1,5);

            for(int i = 0; i< formCount; i++)
            {
                FormModel f = new FormModel();
                f.Time = ViewBag.LastDur / formCount;
                f.Name = "Оформление заказа";
                forms.Add(f);
            }
            for (int i = 0; i < secCount+4; i++)
            {
                SecModel f = new SecModel();
                f.Time = ViewBag.LastDur / (secCount+2) + rnd.Next(1, 20);
                int r = (rnd.Next(3, 14));
                f.Name = db.Sections.Where(s=>s.SectionId == r).FirstOrDefault().Name;
                secs.Add(f);
                if(i< (secCount + 4)/2) secs2.Add(f);
            }

            ViewBag.LastSec = "";
            ViewBag.LastForm = "";
            ViewBag.MSec = "";
            foreach (var t in secs)
            {
                ViewBag.LastSec += t.Name + ':' + t.Time + "; ";
            }
            foreach (var t in secs2)
            {
                ViewBag.MSec += t.Name + ':' + t.Time + "; ";
            }
            foreach (var t in forms)
            {
                ViewBag.LastForm += t.Name + ':' + t.Time + "; ";
            }


            int del = rnd.Next(6, 10);
            int mStart = 123; // lastSession.StartTime - (lastSession.StartTime / del); ПЕРЕДЕЛАТЬ
            ViewBag.MStart = (mStart / 3600).ToString() + ':' + ((mStart % 3600) / 60).ToString() + ':' + (mStart % 60).ToString();

            int mdur = ViewBag.LastDur - (ViewBag.LastDur / del);
            int mfinish = mStart + mdur;
            ViewBag.MFinish = (mfinish / 3600).ToString() + ':' + ((mfinish % 3600) / 60).ToString() + ':' + (mfinish % 60).ToString();
            ViewBag.MDur = mdur;
            return View();
        }

       



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

