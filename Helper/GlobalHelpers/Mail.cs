using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Web;

namespace Helper.GlobalHelpers
{
    public static class Mail
    {
        public static void SendMail(string subject, string body)
        {
            string from = ConfigurationManager.AppSettings["mailfrom"];
            string to = ConfigurationManager.AppSettings["mailto"];
            string smtp = ConfigurationManager.AppSettings["smtp"];
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress(from);
            if (to.Substring(to.Length - 1, 1) == ";")
                to = to.Substring(0, to.Length - 1);
            string[] toList = to.Split(';');


            foreach (string toAddress in toList)
            {
                mail.To.Add(toAddress);
            }
            SmtpClient client = new SmtpClient();
            //            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = smtp;
            mail.Subject = subject;
            mail.Body = body;
            client.Send(mail);
        }
    }
}