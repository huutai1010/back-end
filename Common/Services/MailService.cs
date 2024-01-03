using Common.AppConfiguration;
using Common.Interfaces;
using Common.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        public MailService(IOptions<MailSettings> mailSettingsOptions)
        {
            _mailSettings = mailSettingsOptions.Value;
        }

        public bool SendMail(MailData mailData)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(_mailSettings.SenderEmail);
                mail.To.Add(mailData.EmailToName);
                mail.Subject = "Wellcome To ETravel";
                mail.Body = $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n<head>\r\n  <meta charset=\"UTF-8\">\r\n  <title>Your login details for eTravel</title>\r\n</head>\r\n<body>\r\n\r\n  <div style=\"width: 600px; margin: 0 auto;\">\r\n\r\n     <img src=\"https://firebasestorage.googleapis.com/v0/b/capstoneetravel-d42ad.appspot.com/o/logo%2FR%20eTravel-01.png?alt=media&token=299eac2c-3b9d-46b0-9a1f-616120fbec60\" alt=\"etravel logo\">\r\n\r\n    <p>\r\n      Hi {mailData.FullName},\r\n    </p>\r\n\r\n    <p>\r\n      Welcome to become a new staff in eTravel!\r\n    </p>\r\n\r\n    <p>\r\n      To get started, you'll need to login an account. Here are your login details:\r\n    </p>\r\n\r\n    <ul>\r\n      <li>Username: {mailData.UserName}</li>\r\n      <li>Password: {mailData.Password}</li>\r\n    </ul>\r\n\r\n    <p>\r\n     Login your account by clicking on the following link:\r\n    </p>\r\n\r\n    <a href=\"https://etravel.azurewebsites.net/\">https://etravel.azurewebsites.net/</a>\r\n\r\n    <p>\r\n      Once you've login your account, you'll be able to access all of the features that eTravel has to offer.\r\n    </p>\r\n\r\n    <p>\r\n      We hope you enjoy working on eTravel!\r\n    </p>\r\n\r\n    <p>\r\n      Sincerely,\r\n    </p>\r\n\r\n    <p>\r\n      Lê Minh Quân\r\n    </p>\r\n\r\n    \r\n\r\n  </div>\r\n\r\n</body>\r\n</html>";
                mail.IsBodyHtml = true;
                // mail.Attachments.Add(new Attachment("C:\\file.zip"));

                using (SmtpClient smtp = new SmtpClient(_mailSettings.Server, _mailSettings.Port))
                {
                    smtp.Credentials = new NetworkCredential(_mailSettings.SenderEmail, _mailSettings.Password);
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CreateTestMessage3()
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("capstoneetravel@gmail.com");
                mail.To.Add("hoangquan9851@gmail.com");
                mail.Subject = "Hello World";
                mail.Body = "<h1>Hello</h1>";
                mail.IsBodyHtml = true;
               // mail.Attachments.Add(new Attachment("C:\\file.zip"));

                using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("capstoneetravel@gmail.com", "psjo eirl kjmc tjrv");
                    smtp.EnableSsl = true;
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch(Exception ex)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
