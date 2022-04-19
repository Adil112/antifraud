using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Neuro
{
    public class Generator
    {
        // !!!! Это повесить на кнопку какую то
        public void GenerateData() // генерить данные для н пользователей, сохраняет в бд, и отправляет на оценку
        {
            antifraudContext db = new antifraudContext();
            Random rnd = new Random();
            int a = rnd.Next(3, 7);

            Guid[] users = new Guid[a];
            for(int i = 0; i < a; i++)
            {
                var usersss = db.Users.ToList();
                var num = usersss.Count();
                int a2 = rnd.Next(1, num);
                users[i] = usersss[num - a2].UserId;
            }
            for(int i =0; i < users.Length; i++)
            {
                var nowTime = DateTime.Now;

                DateTime start = nowTime.AddMinutes(-9);
                DateTime finish = nowTime;

                for (int j =0; j< a; j++)
                {

                    FormTime formTime = new FormTime();
                    //formTime.FormTimeId = rnd.Next(100, 214748300);
                    formTime.Time = rnd.Next(1, 1200);
                    formTime.Form = 1;
                    using (antifraudContext dba = new antifraudContext())
                    {
                        dba.FormTimes.Add(formTime);
                        dba.SaveChanges(); // если что using использовать
                    };
                        

                    SectionTime sectionTime = new SectionTime();
                    //sectionTime.SectionTimeId = rnd.Next(100, 214748300);
                    sectionTime.Time = rnd.Next(1, 1200);
                    sectionTime.Section = rnd.Next(4, 14);

                    using (antifraudContext dba = new antifraudContext())
                    {
                        dba.SectionTimes.Add(sectionTime);
                        dba.SaveChanges();
                    }
                        

                    Session session = new Session();
                    session.Form = formTime.FormTimeId;
                    session.Section = sectionTime.SectionTimeId;
                    session.Pk = a % 2 == 0 ? true : false;
                    session.Country = (byte)rnd.Next(1, 40);
                    session.Users = users[i];
                    session.StartTime = start;
                    session.FinishTime = finish;
                    session.SessionId = Guid.NewGuid();

                    using (antifraudContext dba = new antifraudContext())
                    {
                        dba.Sessions.Add(session);
                        dba.SaveChanges();
                    }
                       
                }
            }
            Evaluate e = new Evaluate();
            e.EvaluateData(users);
        }
    }
}
