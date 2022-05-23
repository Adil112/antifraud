using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.MyModels;
using WebAPI.ProcessNeuro;

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
                    newUser.Surname = data.Surname;
                    newUser.Name = data.Name;
                    newUser.Patronymic = data.Patronymic;
                    var rnd = new Random();
                    newUser.Mark = 0;
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                foreach (var ses in data.sessions) // перебираем все сессии
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
                        sec.StartTime = ses.startTime;
                        sec.FinishTime = ses.finishTime;
                        sec.SessionId = Guid.NewGuid();
                        sec.Users = data.userID;
                        sec.Value = ses.value;
                        if (i < ft.Length) sec.Form = ft[i];
                        else sec.Form = null;
                        if (i < st.Length) sec.Section = st[i];
                        else sec.Section = null;

                        if (ses.device == 0) sec.Device = null;
                        else sec.Device = ses.device;
                        if (ses.browser == 0) sec.Browser = null;
                        else sec.Browser = ses.browser;
                        if (ses.language == 0) sec.Language = null;
                        else sec.Language = ses.language;
                        if (ses.location == 0) sec.Location = null;
                        else sec.Location = ses.location;
                        if (ses.provider == 0) sec.Provider = null;
                        else sec.Provider = ses.provider;
                        sec.Proxy = ses.proxy;
                        sec.Vpn = ses.vpn;
                        if (ses.system == 0) sec.System = null;
                        else sec.System = ses.system;

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
