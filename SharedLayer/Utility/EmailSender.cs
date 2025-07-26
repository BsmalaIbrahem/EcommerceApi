using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SharedLayer.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new System.Net.NetworkCredential("bosiebrahem487@gmail.com", "yixy brdi xfvs gwip")
            };

            return client.SendMailAsync(
                new MailMessage(from: "bosiebrahem487@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                                )
                {
                    IsBodyHtml = true
                });
        }
    }
}
