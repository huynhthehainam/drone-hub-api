using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using MiSmart.Infrastructure.Settings;
using Microsoft.Extensions.Options;
namespace MiSmart.Infrastructure.Services
{
    public class EmailService
    {
        private SmtpSettings smtpSettings;
        private EmailSettings emailSettings;
        public EmailService(IOptions<SmtpSettings> options, IOptions<EmailSettings> options1)
        {
            this.smtpSettings = options.Value;
            this.emailSettings = options1.Value;
        }
        public Task SendMailAsync(String[] receivedUsers, String[] cCedUsers, String[] bCCedUsers, String subject, String body, Boolean isBodyHtml = false, String senderName = null, String from = null)
        {
            return Task.Run(() =>
            {
                senderName = senderName ?? emailSettings.SenderName;
                from = from ?? emailSettings.FromEmail;
                subject = subject ?? "";
                body = body ?? "";
                MailMessage mailMessage = new MailMessage();
                if (String.IsNullOrWhiteSpace(senderName))
                    mailMessage.From = new MailAddress(from);
                else
                    mailMessage.From = new MailAddress(from, senderName);
                foreach (var receivedUser in receivedUsers)
                {
                    mailMessage.To.Add(receivedUser);
                }
                foreach (var cCedUser in cCedUsers)
                {
                    mailMessage.CC.Add(cCedUser);
                }
                foreach (var bCCedUser in bCCedUsers)
                {
                    mailMessage.CC.Add(bCCedUser);
                }
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = isBodyHtml;
                using (var client = new SmtpClient(smtpSettings.Server))
                {
                    client.Port = smtpSettings.Port;
                    client.Credentials = new NetworkCredential(smtpSettings.UserName, smtpSettings.Password);
                    client.EnableSsl = smtpSettings.EnableSsl;
                    client.Send(mailMessage);
                }
            });
        }
    }
}