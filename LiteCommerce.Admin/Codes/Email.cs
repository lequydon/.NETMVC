using LiteCommerce.BusinessLayers;
using LiteCommerce.DomainModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Configuration;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace LiteCommerce.Admin
{
    public class Email
    {
        public static void sendMail(string callbackUrl, string email)
        {
            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string from = section.From;
            string host = section.Network.Host;
            int port = section.Network.Port;
            bool enableSsl = section.Network.EnableSsl;
            string users = section.Network.UserName;
            string password = section.Network.Password;
            MailMessage msg = new MailMessage();
            msg.From = new MailAddress(from, "Sinh vien");
            msg.To.Add(new MailAddress(email));
            msg.Subject = " Reset Password ";
            //msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("", null, MediaTypeNames.Text.Plain));
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(getContentEmail(callbackUrl, email), null, MediaTypeNames.Text.Html));

            SmtpClient smtpClient = new SmtpClient(host, port);
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(from, password);
            smtpClient.Credentials = credentials;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.Send(msg);
        }
        public static string getContentEmail(string callbackUrl, string email)
        {
            Account account = new Account();
            account.Email = email;
            var name = AccountBLL.Account_GetEmployee(account).FirstName + " " + AccountBLL.Account_GetEmployee(account).LastName;
            // StreamReader sr = File.OpenText(Server.MapPath("~/TemplateEmail/EmailCustomer"));
            string strContents = System.IO.File.ReadAllText(HttpContext.Current.Server.MapPath(@"~/Views/Forgetpassword.html"));
            //strContents = strContents.Replace("{Date}", calendar.Date.ToString("MM/dd/yyyy HH:mm"));
            strContents = strContents.Replace("{email}", email);
            strContents = strContents.Replace("{Name}", name);
            strContents = strContents.Replace("{rspass}", callbackUrl);
            //sr.Close();
            return strContents;
        }
    }
}