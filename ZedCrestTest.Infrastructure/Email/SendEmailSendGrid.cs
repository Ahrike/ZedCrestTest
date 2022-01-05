using System;
using System.IO;
using System.Threading.Tasks;
using Application.DTOS;
using Application.Interfaces;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Infrastructure.Email
{
    public class SenderEmailSendGrid : ISendEmailServiceA
    {
        private readonly IOptions<SendGridSettings> _settings;
        public SenderEmailSendGrid(IOptions<SendGridSettings> settings)
        {
            _settings = settings;
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            string mailText = GetHtmlTemplate(mailRequest);
            var client = new SendGridClient(_settings.Value.Key);
            var msg = new SendGridMessage
            {
                From = new EmailAddress(_settings.Value.SenderEmail, _settings.Value.User),
                Subject = "Welcome to ZedCrest",
                PlainTextContent = mailText,
                HtmlContent = mailText,
            };
            foreach (var file in mailRequest.Documents)
            {
                var fileBase64 = Convert.ToBase64String(file.DocumentBytes);
                msg.AddAttachment(file.DocumentName, fileBase64, file.DocumentType);
            }
           
            msg.AddTo(new EmailAddress(mailRequest.ToEmail));
            msg.SetClickTracking(false, false);

            var response  = await client.SendEmailAsync(msg);
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