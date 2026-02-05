using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

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
                Address = _configuration["Email:Address"] ?? throw new Exception("Can't Find Email address"),
                Password = _configuration["Email:Password"] ?? throw new Exception("Can't Find Email password"),
                Host = _configuration["Email:Host"] ?? throw new Exception("Can't Find Email host"),
                Port = int.Parse(_configuration["Email:Port"] ?? throw new Exception("Can't Find Email port"))
            };
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            EmailConfig from = GetEmailConfig();
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(from.Address),
                Subject = subject,
                Body = $"<html><body> {htmlMessage}</body></html>",
                IsBodyHtml = true,
            };
            mailMessage.To.Add(email);
            try
            {
                using (SmtpClient smtpClient = new SmtpClient(from.Host, from.Port)) // Corrected: use host from config
                {
                    smtpClient.Credentials = new NetworkCredential(from.Address, from.Password);
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(mailMessage);
                }

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to send email.", ex);
            }
        }
    }
}
