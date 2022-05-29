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
                users = db.Users.Where(s => s.Email.Contains(search)).ToList();
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
            var user = db.Users.Where(s => s.UserId == id).FirstOrDefault();
            if (user == null) return Redirect("~/Home/Index");
            ViewBag.UserId = id;
            ViewBag.UserFio = user.Surname + ' ' + user.Name + ' ' + user.Patronymic;
            ViewBag.UserMark = user.Mark;
            string flag = "";
            if (user.Mark <= 30) flag = "Зеленый";
            else if (user.Mark <= 75 && user.Mark > 30) flag = "Желтый";
            else flag = "Красный";
            ViewBag.Flag = flag;


            var ses = db.Sessions.Where(s => s.Users == id).ToList();
            List<int> loc = new List<int>();
            List<int> sys = new List<int>();
            List<int> providers = new List<int>();

            UserSes userses = new UserSes();
            userses.first = ses.OrderByDescending(s => s.StartTime).First().StartTime;
            userses.last = ses.OrderBy(s => s.StartTime).First().StartTime;
            userses.count = 0;

            List<DateTime> ids = new List<DateTime>();
            foreach(var t in ses)
            {
                if(ids.Count == 0)
                {
                    ids.Add(t.StartTime);
                    continue;
                }
                if(!ids.Contains(t.StartTime))
                {
                    ids.Add(t.StartTime);
                    userses.count += 1;
                }
            }



            List<SesSimple> sessimple = new List<SesSimple>();
            foreach(var t in ids)
            {
                SesSimple s = new SesSimple();
                s.start = t;
                var minses = ses.Where(s => s.StartTime == t);
                s.finish = minses.First().FinishTime;
                s.device = db.Devices.Where(r=>r.DeviceId == minses.First().Device).FirstOrDefault().Name;
                var minloc = db.Locations.Where(r => r.LocationId == minses.First().Location).FirstOrDefault();
                s.location = minloc.Country + ", " + minloc.City;
                s.value = (int)minses.First().Value;
                s.browser = db.Browsers.Where(r => r.BrowserId == minses.First().Browser).FirstOrDefault().Name;
                s.provider = db.Providers.Where(r => r.ProviderId == minses.First().Provider).FirstOrDefault().Name;
                s.system = db.Systems.Where(r => r.SystemId == minses.First().System).FirstOrDefault().Name;
                s.language = db.Languages.Where(r => r.LanguageId == minses.First().Language).FirstOrDefault().Name;
                s.vpn = (bool)minses.First().Vpn ? "Да" : "Нет";
                s.proxy = (bool)minses.First().Proxy ? "Да" : "Нет";
                string minforms = "";
                string minsections = "";
                foreach (var item in minses)
                {
                    var f = db.FormTimes.Where(r => r.FormTimeId == item.Form).FirstOrDefault();
                    var sss = db.SectionTimes.Where(r => r.SectionTimeId == item.Section).FirstOrDefault();
                    minforms += f.Form.ToString() + '(' + f.Time.ToString() +')' + ' ';
                    minsections += sss.Section.ToString() + '(' + f.Time.ToString() + ')' + ' ';
                }
                s.forms = minforms;
                s.sections = minsections;


                sessimple.Add(s);
            }

            ViewBag.SesSimple = sessimple;




            ViewBag.UserSes = userses;


            foreach (var t in ses)
            {
                if(t.Location != null)
                {
                    bool locbool = true;
                    if (loc.Count == 0) loc.Add((int)t.Location);
                    else
                    {
                        foreach(var r in loc)
                        {
                            if (r == t.Location) locbool = false;
                        }
                        if(locbool) loc.Add((int)t.Location);
                    }
                }
                if (t.System != null)
                {
                    bool sysbool = true;
                    if (sys.Count == 0) sys.Add((int)t.System);
                    else
                    {
                        foreach (var r in sys)
                        {
                            if (r == t.System) sysbool = false;
                        }
                        if (sysbool) sys.Add((int)t.System);
                    }
                }
                if (t.Provider != null)
                {
                    bool probool = true;
                    if (providers.Count == 0) providers.Add((int)t.Provider);
                    else
                    {
                        foreach (var r in providers)
                        {
                            if (r == t.Provider) probool = false;
                        }
                        if (probool) providers.Add((int)t.Provider);
                    }
                }


            }

            List<string> locs = new List<string>();
            List<string> syss = new List<string>();
            List<string> providerss = new List<string>();

            foreach (var tt in loc)
            {
                var e = db.Locations.Where(s => s.LocationId == tt).FirstOrDefault();
                string locname = e.Country + ':' + e.City;
                locs.Add(locname);
            }
            foreach (var tt in sys)
            {
                var e = db.Systems.Where(s => s.SystemId == tt).FirstOrDefault();
                string sysname = e.Name;
                syss.Add(sysname);
            }
            foreach (var tt in providers)
            {
                var e = db.Providers.Where(s => s.ProviderId == tt).FirstOrDefault();
                string proname = e.Name;
                providerss.Add(proname);
            }

            ViewBag.Loc = locs;
            ViewBag.Sys = syss;
            ViewBag.Pro = providerss;


            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Stats()
        {
            return View();
        }
        public IActionResult StatUser()
        {
            return View();
        }
        public JsonResult JsonData()
        {
            var data = Charts.PieToday.MultiLineData();
            return Json(data);
        }

        public JsonResult JsonData2()
        {
            var data = Charts.PieToday.PieData();
            return Json(data);
        }

        public JsonResult JsonData3()
        {
            var data = Charts.PieToday.UserData();
            return Json(data);
        }

        public class UserSes
        {
            public DateTime first { get; set; }
            public DateTime last { get; set; }
            public int count { get; set; }

        }

        public class SesSimple
        {
            public DateTime start { get; set; }
            public DateTime finish { get; set; }
            public string device { get; set; }
            public string location { get; set; }
            public string forms { get; set; }
            public string sections { get; set; }
            public int value { get; set; }
            public string browser { get; set; }
            public string provider { get; set; }
            public string system { get; set; }
            public string language { get; set; }
            public string vpn { get; set; }
            public string proxy { get; set; }
        }
    }
}

