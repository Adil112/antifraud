using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;
using WebAPI.Neuro;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        antifraudContext db = new antifraudContext();
        [HttpPost]
        public async Task<ActionResult<List<DataAPI>>> SendData(DataAPI dataAPI) // для одного, после можно для нескольких сделать если хош
        {
            Guid[] userData = new Guid[dataAPI.users.Count];
            int userCounter = 0;
            foreach (var data in dataAPI.users)
            {
                userData[userCounter] = data.userID;
                userCounter++;
                var user = db.Users.Where(s => s.UserId == data.userID).FirstOrDefault();
                if (user == null) //проверка на нового пользователя
                {
                    User newUser = new User();
                    newUser.UserId = data.userID;
                    newUser.Email = data.email;
                    newUser.Fio = data.FIO;
                    newUser.Mark = 0;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                foreach(var ses in data.sessions) // перебираем все сессии
                {
                    int[] ft = new int[ses.forms.Count];
                    int[] st = new int[ses.sections.Count];
                    int fc = 0, sc = 0;
                    foreach (var t in ses.forms) // добавляем формы
                    {
                        FormTime item = new FormTime();
                        item.Form = t.formId;
                        item.Time = t.time;
                        db.FormTimes.Add(item);
                        db.SaveChanges();

                        ft[fc] = db.FormTimes.Where(s => s.Form == item.Form).Where(r => r.Time == item.Time).FirstOrDefault().FormTimeId;
                        fc++;
                    }
                    foreach (var t in ses.sections) // добавляем секции
                    {
                        SectionTime item = new SectionTime();
                        item.Section = t.sectionId;
                        item.Time = t.time;
                        db.SectionTimes.Add(item);
                        db.SaveChanges();

                        st[sc] = db.SectionTimes.Where(s => s.Section == item.Section).Where(r => r.Time == item.Time).FirstOrDefault().SectionTimeId;
                        sc++;
                    }
                    for (int i = 0; i < st.Length; i++) // добавляем сессии
                    {
                        Session sec = new Session();
                        sec.Country = (byte)ses.country;
                        sec.Pk = ses.pk;
                        sec.StartTime = ses.startTime;
                        sec.FinishTime = ses.finishTime;
                        sec.SessionId = Guid.NewGuid();
                        sec.Users = data.userID;
                        sec.Section = st[i];
                        sec.Value = ses.value;
                        if (i < ft.Length) sec.Form = ft[i];
                        else sec.Form = 1;

                        db.Sessions.Add(sec);
                        db.SaveChanges();
                    }
                }  
            }
            Evaluate ev = new Evaluate();
            resultAPI[] res = ev.EvaluateData(userData);
            //переделать, создать отд класс модель для этого
            return Ok(res); // возвращаем
        }
    }
}
