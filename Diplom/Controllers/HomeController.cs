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
            else if (user.Mark <= 75 && user.Mark > 30) flag = "Желтый";
            else  flag = "Красный";
            ViewBag.Flag = flag;

            var nowT = DateTime.Now;

            ViewBag.LastStart = 0;
            ViewBag.LastFinish = 0;
            ViewBag.LastDur = 0;
            ViewBag.LastCountry = 0;
            ViewBag.LastPK = 0;
            var lastSession = db.Sessions.Where(r=>r.Users == id).OrderByDescending(r=>r.FinishTime).Where(s=>s.FinishTime < nowT).FirstOrDefault();
            if(lastSession != null)
            {
                ViewBag.LastStart = lastSession.StartTime;
                ViewBag.LastFinish = lastSession.FinishTime;
                var duration = lastSession.FinishTime - lastSession.StartTime;
                ViewBag.LastDur = (int)duration.TotalMinutes;
                ViewBag.LastCountry = db.Countries.Where(s => s.CountryId == lastSession.Country).FirstOrDefault().Name;
                ViewBag.LastPK = "Нет";
                if (lastSession.Pk) ViewBag.LastPK = "Да";
            }
            
           List <FormModel> forms = new List<FormModel>();
           List<SecModel> secs = new List<SecModel>();
           List<SecModel> secs2 = new List<SecModel>();

            Random rnd = new Random();
            int formCount = rnd.Next(2,3);
            int secCount = rnd.Next(3,8);

            for(int i = 0; i< formCount; i++)
            {
                FormModel f = new FormModel();
                f.Time = (int)ViewBag.LastDur / (int)formCount;
                int r = (rnd.Next(1, 7));
                f.Name = db.Forms.Where(s => s.FormId == r).FirstOrDefault().Name;
                forms.Add(f);
            }
            for (int i = 0; i < secCount+4; i++)
            {
                SecModel f = new SecModel();
                f.Time = (int)(ViewBag.LastDur / (secCount+2) + rnd.Next(1, 20));
                int r = (rnd.Next(1, 12));
                f.Name = db.Sections.Where(s=>s.SectionId == r).FirstOrDefault().Name;
                secs.Add(f);
                if(i< (secCount + 4)/2) secs2.Add(f);
            }

            ViewBag.LastSec = "";
            ViewBag.LastForm = "";
            ViewBag.MSec = "";
            ViewBag.MForm = "";
            foreach (var t in secs)
            {
                ViewBag.LastSec += t.Name + ':' + t.Time + "; ";
            }
            foreach (var t in secs2)
            {
                ViewBag.MSec += t.Name + ':' + (t.Time + rnd.Next(0,5)) + "; ";
            }
            var ppp = 0;
            foreach (var t in forms)
            {
                ViewBag.LastForm += t.Name + ':' + t.Time + "; ";
                if (ppp == 0) ViewBag.MForm += t.Name + ':' + (t.Time + rnd.Next(2, 5)) + "; ";
                ppp++;
            }


            var sessions = db.Sessions.Where(s => s.Users == id);
            int mStart = 0;
            int mFinish = 0;
            foreach(var t in sessions)
            {
                mStart += t.StartTime.Hour * 60;
                mFinish += t.FinishTime.Hour * 60;
            }

            int start = mStart / sessions.Count();
            int finish = mFinish / sessions.Count();

            if(finish > start)
            {
                ViewBag.MStart = (start / 60).ToString() + ':' + (start % 60).ToString() + ":00";
                ViewBag.MFinish = (finish / 60).ToString() + ':' + (finish % 60).ToString() + ":00";
            }
            else
            {
                ViewBag.MStart = (finish / 60).ToString() + ':' + (finish % 60).ToString() + ":00"; 
                ViewBag.MFinish = (start / 60).ToString() + ':' + (start % 60).ToString() + ":00";
            }
            
            int mdur = 0;
            if(mFinish > mStart) mdur = mFinish - mStart;
            else mdur = mStart - mFinish;

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

