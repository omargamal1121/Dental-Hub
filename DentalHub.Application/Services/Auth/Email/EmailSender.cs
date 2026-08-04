using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace DentalHub.Application.Services.Auth
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private class EmailConfig
        {
            public string Address { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
        }

        private EmailConfig GetEmailConfig()
        {
            return new EmailConfig
            {
                Address = _configuration["Email:Address2"] ?? throw new Exception("Can't Find Email address"),
                Password = _configuration["Email:Password2"] ?? throw new Exception("Can't Find Email password"),
                Host = _configuration["Email:Host"] ?? throw new Exception("Can't Find Email host"),
                Port = int.Parse(_configuration["Email:Port"] ?? throw new Exception("Can't Find Email port"))
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            EmailConfig from = GetEmailConfig();
           
           
            try
            {
				var message = new MimeMessage();
				message.From.Add(MailboxAddress.Parse(from.Address));
				message.To.Add(MailboxAddress.Parse(email));
				message.Subject = subject;
				message.Body = new BodyBuilder { HtmlBody = $"<html><body>{htmlMessage}</body></html>" }.ToMessageBody();


				using var smtp = new SmtpClient();
				await smtp.ConnectAsync(from.Host, from.Port, SecureSocketOptions.StartTls);
				await smtp.AuthenticateAsync(from.Address, from.Password);
				await smtp.SendAsync(message);
				await smtp.DisconnectAsync(true);
			}
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
