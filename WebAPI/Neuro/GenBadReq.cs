using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Neuro
{
    public class GenBadReq
    {
        public DataNeuro[] GenerateBad(DataNeuro[] data)
        {
            int num = (int)(data.Length * 0.2);
            DataNeuro[] newData = new DataNeuro[data.Length + num];
            DataNeuro[] badData = new DataNeuro[num]; // записи с высокой вероятностью взлома
            for(int i = 0; i<badData.Length; i++)
            {
                badData[i] = new DataNeuro();
            }

            int hourStartSum = 0;
            int durationSum = 0;
            List<int> countries = new List<int>(); // список стран
            int pkGNum = 0;
            int pkBNum = 0;
            FormCounter[] fCount = new FormCounter[7];
            SectionCounter[] sCount = new SectionCounter[12];

            for(int i =0; i< fCount.Length; i++)
            {
                FormCounter fc = new FormCounter();
                fc.id = i+1;
                fc.time = 0;
                fc.counter = 0;
                fCount[i] = fc;
            }
            for (int i = 0; i < sCount.Length ; i++)
            {
                SectionCounter fc = new SectionCounter();
                fc.id = i+1;
                fc.time = 0;
                fc.counter = 0;
                sCount[i] = fc;
            }

            for (int i =1; i<47; i++)
            {
                countries.Add(i);
            }
            for(int i =0; i<data.Length; i++)
            {
                hourStartSum += data[i].startTime.Hour;
                var start = data[i].startTime.Hour * 60 + data[i].startTime.Minute;
                var finish = data[i].finishTime.Hour * 60 + data[i].finishTime.Minute;

                if (start > finish) durationSum += (1440 - start) + finish;
                else durationSum += finish - start;
                var country = data[i].country;
                countries.Remove(country);
                if (data[i].pk) pkGNum++;
                else pkBNum++;

                fCount[data[i].form - 1].counter++;
                fCount[data[i].form - 1].time = data[i].formTime;

                sCount[data[i].section - 1].counter++;
                sCount[data[i].section - 1].time = data[i].sectionTime;

            }
            List<FormCounter> formNew = new List<FormCounter>();
            List<SectionCounter> sectionNew = new List<SectionCounter>();

            for(int i = 0; i<fCount.Length; i++)
            {
                if (fCount[i].counter == 0) formNew.Add(fCount[i]);
            }
            for (int i = 0; i < sCount.Length; i++)
            {
                if (sCount[i].counter == 0) sectionNew.Add(sCount[i]);
            }

            bool pkForBad = true; // показывает с чего чаще пользователь заходит
            if (pkGNum > pkBNum) pkForBad = true;
            else pkForBad = false;
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


            d.startTime = DateTime.Now.AddDays(-rnd.Next(1, 20));
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

            int rndCountry = rnd.Next(1, countries.Count);
            d.country = countries[rndCountry];

            if (pkForBad) d.pk = false;
            else d.pk = true;

            for (int i = 0; i<badData.Length; i++)
            {
                DataNeuro newd = d;
                newd.form = formNew[rnd.Next(0, formNew.Count)].id;
                newd.formTime = durationSum / (rnd.Next(5, 12));
                newd.section = sectionNew[rnd.Next(0, sectionNew.Count)].id;
                newd.sectionTime = durationSum / (rnd.Next(5, 12));
                newd.value = rnd.Next(70, 90);
                badData[i] = newd;
            }

            for(int i =0; i< data.Length; i++)
            {
                newData[i] = data[i];
            }
            for (int i = data.Length; i < data.Length+num; i++)
            {
                newData[i] = badData[i-data.Length];
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
