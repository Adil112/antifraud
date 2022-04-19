using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;

namespace WebAPI.Notification
{
    public class Notify
    {
        public void Notificate(string email, string fio)
        {
            string message = "Добрый день, " + fio + "! Прошу проверьте ваш личный кабинет на возможность взлома, " +
                             "так как наша система заметила подозрительное поведение с вашего аккаунта";
            string frompas = "lnpqlhpswvortxxu";
            MailAddress from = new MailAddress("adil.200191@gmail.com", "Adil");
            MailAddress to = new MailAddress("ami5@tpu.ru");
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Тест";
            m.Body = message;
            m.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("adil.200191@gmail.com", frompas);
            smtp.EnableSsl = true;
            smtp.Send(m);
        }
    }
}
