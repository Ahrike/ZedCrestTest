using Application.DTOS;
using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.IO;
using System.Threading.Tasks;

namespace Infrastructure.Email
{
    public class SendEmail: ISendEmailServiceB
    {
        private readonly IOptions<MailSettings> _mailSettings;
        public SendEmail(IOptions<MailSettings> settings)
        {
            _mailSettings = settings;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Value.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = $"Welcome to ZedCrest {mailRequest.UserName}";
            string MailText = GetHtmlTemplate(mailRequest);
            var builder = new BodyBuilder();
            if (mailRequest.Documents != null)
            {
                foreach (var file in mailRequest.Documents)
                {
                    builder.Attachments.Add(file.DocumentName, file.DocumentBytes, ContentType.Parse(file.DocumentType));
                }
            }
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Value.Host, _mailSettings.Value.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Value.Mail, _mailSettings.Value.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
        private static string GetHtmlTemplate(MailRequest mailRequest)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates/WelcomeTemplate.html");
            using (StreamReader str = new StreamReader(filePath))
            {
                string mailText = str.ReadToEnd();
                str.Close();
                mailText = mailText.Replace("[username]", mailRequest.UserName);
                return mailText;
            }
        }
    }
}
