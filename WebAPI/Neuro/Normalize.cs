using System;
using System.Collections.Generic;
using System.Linq;
using WebAPI.Models;

namespace WebAPI.Neuro
{
    public class Normalize
    {
        public NormalizedData[] NormalizeData(Guid idUser, bool time) // time : true = за посл 10 мин : false = исторические данные
        {
            Guid id = idUser;
            antifraudContext db = new antifraudContext();
 
            var nowTime = DateTime.Now;
            var tenTime = nowTime.AddMinutes(-10);
            List<Session> sessions = new List<Session>();
            if(time) sessions = db.Sessions.Where(s => s.Users == id).Where(s => s.StartTime >= tenTime).ToList();
            else sessions = db.Sessions.Where(s => s.Users == id).Where(s => s.StartTime <= tenTime ).ToList();

            var num = sessions.Count();
            DataNeuro[] data = new DataNeuro[num];
            for(int i =0; i<num; i++)
            {
                data[i] = new DataNeuro();
            }
            

            int counter = 0;
            foreach(var t in sessions) // вытаскиваем по айди пользователя все сессии в бд
            {
                if (t.Value != null) data[counter].value = (int)t.Value;
                else data[counter].value = 0;
                data[counter].startTime = t.StartTime;
                data[counter].finishTime = t.FinishTime;
                data[counter].country = t.Country;
                data[counter].pk = t.Pk;
                data[counter].form = db.FormTimes.Where(s => s.FormTimeId == t.Form).FirstOrDefault().Form;
                data[counter].formTime = db.FormTimes.Where(s => s.FormTimeId == t.Form).FirstOrDefault().Time;
                data[counter].section = db.SectionTimes.Where(s => s.SectionTimeId == t.Section).FirstOrDefault().Section;
                data[counter].sectionTime = db.SectionTimes.Where(s => s.SectionTimeId == t.Section).FirstOrDefault().Time;
                counter++;
            }

            if(!time)
            {
                GenBadReq gbr = new GenBadReq();
                data = gbr.GenerateBad(data);
            }

            NormalizedData[] normData = new NormalizedData[data.Length];
            for (int i = 0; i < normData.Length; i++)
            {
                normData[i] = new NormalizedData();
            }

            for (int i =0; i< data.Length; i++) // нормализуем все данные
            {
                var start = data[i].startTime.Hour * 3600 + data[i].startTime.Minute * 60 + data[i].startTime.Second;
                var finish = data[i].finishTime.Hour * 3600 + data[i].finishTime.Minute * 60 + data[i].finishTime.Second;
                normData[i].startTime = Uravnenie(start, 1, 86400);
                normData[i].startTime = Uravnenie(finish, 1, 86400);
                normData[i].country = Uravnenie(data[i].country, 1, db.Countries.Count());
                normData[i].pk = data[i].pk == true ? 1 : 0;

                int cForm = db.Forms.Count();
                int minForm = db.Forms.FirstOrDefault().FormId;
                int maxForm = cForm + minForm;

                int cSec = db.Sections.Count();
                int minSec = db.Sections.FirstOrDefault().SectionId;
                int maxSec = cSec + minSec;

                normData[i].form = Uravnenie(data[i].form, minForm, maxForm);
                normData[i].section = Uravnenie(data[i].section, minSec, maxSec);

                normData[i].formTime = Uravnenie(data[i].formTime, 1, 1200);
                normData[i].sectionTime = Uravnenie(data[i].sectionTime, 1, 1200);

                normData[i].value = 0.01 * data[i].value;
            }
            return normData; //возврашаем массив нормализованных данных в диапозоне 0-1 для нейронной сети всех сессии 1 пользователя
        }

        public double Uravnenie(double x, double min, double max) //уравнение нормализации для диапозона 0-1
        {
            double y = (x - min) / (max - min);
            return y;
        }


    }
}
