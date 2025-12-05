using MailKit.Net.Smtp;
using MimeKit;
using server.Services.Interfaces;

namespace server.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("PMS System", _config["Email:From"]));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;

            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(_config["Email:From"], _config["Email:Password"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
