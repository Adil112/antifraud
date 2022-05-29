using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Diplom.Charts
{
    public static class PieToday
    {
        public static List<object> MultiLineData() // общая за 2 недели
        {
            DateTime now = DateTime.Now;
            var twoWeeks = now.AddDays(-14);
            antifraudContext db = new antifraudContext();
            var sessions = db.Sessions.Where(r => r.StartTime > twoWeeks); // список входов за 2 недели
            var hackSessions = sessions.Where(s => s.Value > 75); // список взломов за 2 недели
            List<LineChartTwoWeeks> data = new List<LineChartTwoWeeks>();
            for(int i =1; i<15; i++)
            {
                var sesByDay = sessions.Where(r=>r.StartTime < now.AddDays(-i)).Where(r=>r.StartTime > now.AddDays(-(i+1)));
                LineChartTwoWeeks line = new LineChartTwoWeeks();
                line.date = now.AddDays(-(i+1)).Day;
                line.enters = sesByDay.Count();
                var sesByDayHack = sesByDay.Where(r => r.Value > 75);
                line.hacks = sesByDayHack.Count();
                data.Add(line);
            }


            List<object> objs = new List<object>();
            objs.Add(new[] { "Дата", "Взлом", "Авторизация"});
            foreach(var t in data)
            {
                objs.Add(new[] { t.date, t.hacks, t.enters });
            }
            return objs;

        }

        public static List<object> PieData() // общая за день
        {
            DateTime now = DateTime.Now;
            var today = now.AddDays(-1);
            antifraudContext db = new antifraudContext();
            var sessions = db.Sessions.Where(r => r.StartTime > today);
            int[] data = new int[10];
            foreach(var t in sessions)
            {   if(t.Value > 10)
                {
                    int risk = (int)(t.Value / 10);
                    data[risk] += 1;
                }
                
            }


            List<object> objs = new List<object>();
            objs.Add(new[] { "Риск", "Инцидент" });
            for(int i =0; i < 10; i++)
            {
                objs.Add(new[] { i*10, data[i] });
            }
            return objs;

        }
        public class LineChartTwoWeeks
        {
            public int date { get; set; }
            public int hacks { get; set; }
            public int enters { get; set; }
        }


        public static List<object> UserData() // за 2 недели 1 пользователь
        {
            DateTime now = DateTime.Now;
            var twoWeeks = now.AddDays(-14);
            antifraudContext db = new antifraudContext();

            var user = db.Users.OrderBy(r => Guid.NewGuid()).First().UserId;
            var sessions = db.Sessions.Where(r => r.StartTime > twoWeeks).Where(s=>s.Users == user); // список входов за 2 недели
            var hackSessions = sessions.Where(s => s.Value > 75); // список взломов за 2 недели
            List<LineChartTwoWeeks> data = new List<LineChartTwoWeeks>();
            for (int i = 1; i < 15; i++)
            {
                var sesByDay = sessions.Where(r => r.StartTime < now.AddDays(-i)).Where(r => r.StartTime > now.AddDays(-(i + 1)));
                LineChartTwoWeeks line = new LineChartTwoWeeks();
                line.date = now.AddDays(-(i + 1)).Day;
                line.enters = sesByDay.Count();
                var sesByDayHack = sesByDay.Where(r => r.Value > 75);
                line.hacks = sesByDayHack.Count();
                data.Add(line);
            }


            List<object> objs = new List<object>();
            objs.Add(new[] { "Дата", "Взлом", "Авторизация" });
            foreach (var t in data)
            {
                objs.Add(new[] { t.date, t.hacks, t.enters });
            }
            return objs;

        }
    }
}
