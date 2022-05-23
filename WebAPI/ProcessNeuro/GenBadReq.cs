using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.MyModels;

namespace WebAPI.ProcessNeuro
{
    public class GenBadReq
    {
        public DataNeuro[] GenerateBad(DataNeuro[] data)
        {
            antifraudContext db = new antifraudContext();

            int LocationNumber = db.Locations.ToList().Count - 1;
            int BrowserNumber = db.Browsers.ToList().Count - 1;
            int ProviderNumber = db.Providers.ToList().Count - 1;
            int LanguageNumber = db.Languages.ToList().Count - 1;
            int SystemNumber = db.Systems.Where(s => s.ComputerAbility == true).ToList().Count - 1;
            int FormNumber = db.Forms.ToList().Count - 1;
            int SectionNumber = db.Sections.ToList().Count - 1;

            int num = (int)(data.Length * 0.2);
            DataNeuro[] newData = new DataNeuro[data.Length + num];
            DataNeuro[] badData = new DataNeuro[num]; // записи с высокой вероятностью взлома
            for (int i = 0; i < badData.Length; i++)
            {
                badData[i] = new DataNeuro();
            }

            int hourStartSum = 0;
            int durationSum = 0;
            List<int> locations = new List<int>(); // список локации
            List<int> browsers = new List<int>(); //список браузеров
            List<int> providers = new List<int>(); // список провайдеров
            List<int> systems = new List<int>(); // список ОС
            List<int> languages = new List<int>(); // список языков
            FormCounter[] fCount = new FormCounter[FormNumber--];
            SectionCounter[] sCount = new SectionCounter[SectionNumber--];

            for (int i = 2; i < fCount.Length+2; i++)
            {
                FormCounter fc = new FormCounter();
                fc.id = i;
                fc.time = 0;
                fc.counter = 0;
                fCount[i-2] = fc;
            }
            for (int i = 2; i < sCount.Length+2; i++)
            {
                SectionCounter fc = new SectionCounter();
                fc.id = i;
                fc.time = 0;
                fc.counter = 0;
                sCount[i-2] = fc;
            }

            for (int i = 2; i < LocationNumber; i++)
            {
                locations.Add(i);
            }
            for (int i = 2; i < BrowserNumber; i++)
            {
                browsers.Add(i);
            }
            for (int i = 2; i < ProviderNumber; i++)
            {
                providers.Add(i);
            }
            for (int i = 2; i < LanguageNumber; i++)
            {
                languages.Add(i);
            }
            for (int i = 2; i < SystemNumber; i++)
            {
                systems.Add(i);
            }
            for (int i = 0; i < data.Length; i++)
            {
                hourStartSum += data[i].startTime.Hour;
                var start = data[i].startTime.Hour * 60 + data[i].startTime.Minute;
                var finish = data[i].finishTime.Hour * 60 + data[i].finishTime.Minute;

                if (start > finish) durationSum += (1440 - start) + finish;
                else durationSum += finish - start;
                var location = data[i].location;
                locations.Remove(location);
                var browser = data[i].browser;
                browsers.Remove(browser);
                var provider = data[i].provider;
                providers.Remove(provider);
                var language = data[i].language;
                languages.Remove(language);
                var system = data[i].system;
                systems.Remove(system);


                fCount[data[i].form - 2].counter++;
                fCount[data[i].form - 2].time = data[i].formTime;

                sCount[data[i].section - 2].counter++;
                sCount[data[i].section - 2].time = data[i].sectionTime;

            }
            List<FormCounter> formNew = new List<FormCounter>();
            List<SectionCounter> sectionNew = new List<SectionCounter>();

            for (int i = 0; i < fCount.Length; i++)
            {
                if (fCount[i].counter == 0) formNew.Add(fCount[i]);
            }
            for (int i = 0; i < sCount.Length; i++)
            {
                if (sCount[i].counter == 0) sectionNew.Add(sCount[i]);
            }


            hourStartSum = (int)(hourStartSum / data.Length); // самое частое время начала сессии
            durationSum = (int)(durationSum / data.Length); // среднее продолжительность сессии


            DataNeuro d = new DataNeuro();
            int minStart = 0;
            int minDur = 0;
            int minFinish = 0;
            Random rnd = new Random();

            if (hourStartSum > 720) minStart = hourStartSum - 720;
            else minStart = hourStartSum + 720;
            minDur = durationSum / 10;
            minFinish = minStart + durationSum;


            d.startTime = DateTime.Now.AddDays(-rnd.Next(1, 30));
            d.startTime.AddHours(minStart / 60);
            d.startTime.AddMinutes(minStart % 60);

            if (minFinish > 1440)
            {
                d.finishTime = d.startTime.AddDays(1);
                minFinish = minFinish - 1440;
                d.finishTime.AddHours(minFinish / 60);
                d.finishTime.AddMinutes(minFinish % 60);
            }
            else
            {
                d.finishTime = d.startTime;
                d.finishTime.AddHours(minFinish / 60);
                d.finishTime.AddMinutes(minFinish % 60);
            }

            int rndLocation = rnd.Next(0, locations.Count - 1);
            d.location = locations[rndLocation];
            int rndBrowser = rnd.Next(0, browsers.Count - 1);
            d.browser = browsers[rndBrowser];
            int rndProvider = rnd.Next(0, providers.Count - 1);
            d.provider = providers[rndProvider];
            int rndSystem = rnd.Next(0, systems.Count - 1);
            d.system = systems[rndSystem];
            int rndLanguage = rnd.Next(0, languages.Count - 1);
            d.language = languages[rndLanguage];

            d.vpn = true;
            d.proxy = true;


            for (int i = 0; i < badData.Length; i++)
            {
                DataNeuro newd = d;
                newd.form = formNew[rnd.Next(0, formNew.Count)].id;
                newd.formTime = durationSum / (rnd.Next(5, 12));
                newd.section = sectionNew[rnd.Next(0, sectionNew.Count)].id;
                newd.sectionTime = durationSum / (rnd.Next(5, 12));
                newd.value = rnd.Next(80, 99);
                if (i % 2 == 0) newd.device = db.Devices.Where(s => s.Name == "Компьютер").FirstOrDefault().DeviceId;
                else newd.device = db.Devices.Where(s => s.Name == "Ноутбук").FirstOrDefault().DeviceId;
                badData[i] = newd;
            }

            for (int i = 0; i < data.Length; i++)
            {
                newData[i] = data[i];
            }
            for (int i = data.Length; i < data.Length + num; i++)
            {
                newData[i] = badData[i - data.Length];
            }

            return newData;
        }

        public class FormCounter
        {
            public int id { get; set; }
            public int time { get; set; }
            public int counter { get; set; }
        }
        public class SectionCounter
        {
            public int id { get; set; }
            public int time { get; set; }
            public int counter { get; set; }
        }


    }
}
