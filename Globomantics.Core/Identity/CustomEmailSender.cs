using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Globomantics.Core.Identity
{
    public class CustomEmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _fromAddress;

        public CustomEmailSender(string smtpServer, int smtpPort, string fromAddress)
        {
            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _fromAddress = fromAddress;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_fromAddress),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(email));
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Send(message);
            }

            return Task.CompletedTask;
        }
    }
}
