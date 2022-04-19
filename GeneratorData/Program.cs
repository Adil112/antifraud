using GeneratorData.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace GeneratorData
{
    class Program
    {
        static void Main(string[] args)
        {
            Random rnd = new Random();
            DateTime now = DateTime.Now;
            DataAPI data = new DataAPI();
            UserAPI user = new UserAPI();
            user.userID = Guid.NewGuid();
            user.FIO = GenFIO();
            user.email = user.FIO + "@gmail.com";
            int country = rnd.Next(1, 44);

            List<sessionApi> ses = new List<sessionApi>();
            sessionApi good = new sessionApi();
            for(int i =0; i < 6; i++)
            {
                sessionApi s = new sessionApi();
                s.country = country;
                s.pk = true;
                int dur = rnd.Next(900, 12000);
                if( i < 5) s.startTime = now.AddDays(-(i + 1) * 2).AddHours(rnd.Next(-5, 5)).AddMinutes(rnd.Next(-30, 30)).AddSeconds(rnd.Next(-30, 30));
                else s.startTime = now.AddMinutes(rnd.Next(-8, -1)).AddSeconds(rnd.Next(-30, 30));
                s.finishTime = s.startTime.AddSeconds(dur);
                s.value = rnd.Next(2,30);

                List<formApi> flist = new List<formApi>();
                List<sectionApi> slist = new List<sectionApi>();

 
                int secNum = rnd.Next(3, 5);

                for (int t = 0; t < secNum; t++)
                {
                    formApi ff = new formApi();
                    ff.formId = rnd.Next(1,7);
                    ff.time = rnd.Next(dur/ 8) + rnd.Next(1, 10);
                    flist.Add(ff);

                    sectionApi ss = new sectionApi();
                    ss.sectionId = rnd.Next(1, 12);
                    ss.time = rnd.Next(dur / 8) + rnd.Next(1, 10);
                    slist.Add(ss);
                }

                s.forms = flist;
                s.sections = slist;
                if (i < 5) ses.Add(s);
                else good = s;

            }
            user.sessions = ses;

            Console.OutputEncoding = Encoding.UTF8;
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            string json = JsonSerializer.Serialize(user, options);
            Console.WriteLine(json);

            Console.WriteLine("Обычная сессия");
            ses.Clear();
            ses.Add(good);
            json = JsonSerializer.Serialize(user, options);
            Console.WriteLine(json);


            //взломанная сессия
            sessionApi badSes = new sessionApi();
            badSes.country = rnd.Next(1, 44);
            badSes.startTime = good.startTime;
            badSes.finishTime = good.finishTime;
            badSes.pk = false;
            badSes.value = 1;
            List<formApi> flist2 = new List<formApi>();
            List<sectionApi> slist2 = new List<sectionApi>();
            int Num = rnd.Next(1, 3);
            int durat = rnd.Next(1, 500);
            for (int t = 0; t < Num; t++)
            {
                formApi ff = new formApi();
                ff.formId = rnd.Next(1, 7);
                ff.time = rnd.Next(durat / 8) + rnd.Next(1, 10);
                flist2.Add(ff);

                sectionApi ss = new sectionApi();
                ss.sectionId = rnd.Next(1, 12);
                ss.time = rnd.Next(durat / 8) + rnd.Next(1, 10);
                slist2.Add(ss);
            }
            badSes.forms = flist2;
            badSes.sections = slist2;

            Console.WriteLine("Взломанная сессия");
            ses.Clear();
            ses.Add(badSes);
            json = JsonSerializer.Serialize(user, options);
            Console.WriteLine(json);
        }

        public static string GenFIO()
        {
            string[] maleNames = new string[10] { "Иван", "Петр", "Сергей", "Николай", "Алексей", "Александр", "Вадим", "Анатолий", "Никита", "Федор" };
            string[] femaleNames = new string[10] { "Алина", "Елена", "Елизавета", "Юлия", "Ксения", "София", "Кристина", "Марина", "Любовь", "Вера"};
            string[] maleSecNames = new string[10] { "Иванов", "Петров", "Сергеев", "Николаев", "Алексеев", "Александров", "Вадимов", "Сидоров", "Печенкин", "Федоров" };
            string[] femaleSecNames = new string[10] { "Синицина", "Дроздова", "Федорко", "Малина", "Часовых", "Малых", "Березова", "Горных", "Чебаткова", "Майор" };
            string[] maleLastNames = new string[10] { "Иванович", "Петрович", "Сергеевич", "Николаевич", "Алексеевич", "Александрович", "Вадимович", "Дмитриевич", "Михаилович", "Евгеньевич"};
            string[] femaleLastNames = new string[10] { "Ивановна", "Петровна", "Сергеевна", "Николаевна", "Алексеевна", "Александровна", "Вадимовна", "Дмитриевна", "Маихайловна", "Евгеньевна" };
            string fio = "";
            Random rand = new Random(DateTime.Now.Second);
            if (rand.Next(1, 2) == 1)
            {
                fio = maleSecNames[rand.Next(0, 9)] + ' ' + maleNames[rand.Next(0, 9)] + ' ' + maleLastNames[rand.Next(0, 9)];
            }
            else
            {
                fio = femaleSecNames[rand.Next(0, 9)] + ' ' + femaleNames[rand.Next(0, 9)] + ' ' + femaleLastNames[rand.Next(0, 9)];
            }
            return fio;
        }
    }
}
