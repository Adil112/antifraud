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
            string[] fio = GenFIO();
            user.surname = fio[0];
            user.name = fio[1];
            user.patronymic = fio[2];
            user.email = user.surname + '_' + user.name + '_' + user.patronymic + "@gmail.com";
            int location = rnd.Next(2, 39);
            int device = rnd.Next(2, 4);
            int provider = rnd.Next(2, 9);
            int system = rnd.Next(2, 16);
            int language = rnd.Next(2, 18);
            int browser = rnd.Next(2, 10);
            int duration = 0;

            List<sessionApi> ses = new List<sessionApi>();
            sessionApi good = new sessionApi();
            for(int i =0; i < 6; i++)
            {
                sessionApi s = new sessionApi();
                s.location = location;
                s.device = device;
                s.provider = provider;
                s.system = system;
                s.language = language;
                s.browser = browser;
                s.proxy = false;
                s.vpn = false;
                duration = rnd.Next(900, 12000);
                if( i < 5) s.startTime = now.AddDays(-(i + 1) * 2).AddHours(rnd.Next(-5, 5)).AddMinutes(rnd.Next(-30, 30)).AddSeconds(rnd.Next(-30, 30));
                else s.startTime = now.AddMinutes(rnd.Next(-8, -1)).AddSeconds(rnd.Next(-30, 30));
                s.finishTime = s.startTime.AddSeconds(duration);
                s.value = rnd.Next(5,65); 

                List<formApi> flist = new List<formApi>();
                List<sectionApi> slist = new List<sectionApi>();

 
                int secNum = rnd.Next(3, 5);

                for (int t = 0; t < secNum; t++)
                {
                    formApi ff = new formApi();
                    ff.formId = rnd.Next(2,7);
                    ff.time = rnd.Next(duration / 8) + rnd.Next(1, 10);
                    flist.Add(ff);

                    sectionApi ss = new sectionApi();
                    ss.sectionId = rnd.Next(2, 13);
                    ss.time = rnd.Next(duration / 8) + rnd.Next(1, 10);
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
            good.startTime = now;
            good.finishTime = now.AddSeconds(duration);
            ses.Add(good);
            json = JsonSerializer.Serialize(user, options);
            Console.WriteLine(json);


            //взломанная сессия
            sessionApi badSes = new sessionApi();
            badSes.location = rnd.Next(2, 39);
            badSes.startTime = good.startTime;
            badSes.finishTime = good.finishTime;
            badSes.device = device--;
            badSes.provider = provider--;
            badSes.system = system--;
            badSes.language = language--;
            badSes.browser = browser--;
            if(rnd.Next(1,2) == 1)
            {
                badSes.proxy = false;
                badSes.vpn = true;
            }
            else
            {
                badSes.proxy = true;
                badSes.vpn = false;
            }
            
            
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

        public static string[] GenFIO()
        {
            string[] fio = new string[3];
            string[] maleNames = new string[20] { "Иван", "Петр", "Сергей", "Николай", "Алексей", "Александр", "Вадим", "Анатолий", "Никита", "Федор", "Даниил", "Петр", "Федор", "Григорий", "Давид", "Максим", "Вячеслав", "Артем", "Родион", "Тимофей" };
            string[] femaleNames = new string[20] { "Алина", "Елена", "Елизавета", "Юлия", "Ксения", "София", "Кристина", "Марина", "Любовь", "Вера", "Кира", "Милана", "Василиса", "Анна", "Мария", "Арина", "Ульяна", "Алиса", "Агния", "Ангелина" };
            string[] maleSecNames = new string[20] { "Иванов", "Петров", "Сергеев", "Николаев", "Алексеев", "Александров", "Вадимов", "Сидоров", "Печенкин", "Федоров", "Богданов", "Логинов", "Жданов", "Горшков", "Буров", "Кононов", "Щеглов", "Назаров", "Виноградов", "Константинов" };
            string[] femaleSecNames = new string[20] { "Синицина", "Дроздова", "Федорко", "Малина", "Часовых", "Малых", "Березова", "Горных", "Чебаткова", "Лаврова", "Зайцева", "Бирюкова", "Карташова", "Мельникова", "Новикова", "Гришина", "Грачева", "Петухова", "Михайлова", "Шарова" };
            string[] maleLastNames = new string[20] { "Иванович", "Петрович", "Сергеевич", "Николаевич", "Алексеевич", "Александрович", "Вадимович", "Дмитриевич", "Михаилович", "Евгеньевич", "Никитиевич", "Улебович", "Онисимович", "Фролович", "Кириллович", "Федосеевич", "Витальевич", "Рудольфович", "Львович", "Степанович" };
            string[] femaleLastNames = new string[20] { "Ивановна", "Петровна", "Сергеевна", "Николаевна", "Алексеевна", "Александровна", "Вадимовна", "Дмитриевна", "Маихайловна", "Евгеньевна", "Леонидовна", "Федосеевна", "Германова", "Платоновна", "Васильевна", "Артемовна", "Макаровна", "Денисовна", "Дамировна", "Вячеславовна" };
            Random rand = new Random(DateTime.Now.Second);
            if (rand.Next(1, 2) == 1)
            {
                fio[0] = maleSecNames[rand.Next(0, 19)];
                fio[1] = maleNames[rand.Next(0, 19)];
                fio[2] = maleLastNames[rand.Next(0, 19)];
            }
            else
            {
                fio[0] = femaleSecNames[rand.Next(0, 19)];
                fio[1] = femaleNames[rand.Next(0, 19)];
                fio[2] = femaleLastNames[rand.Next(0, 19)];
            }
            return fio;
        }
    }
}
