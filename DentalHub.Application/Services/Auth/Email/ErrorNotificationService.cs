using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Auth
{
    public class ErrorNotificationService : IErrorNotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ErrorNotificationService> _logger;

        public ErrorNotificationService(
            IEmailSender emailSender,
            IConfiguration configuration,
            ILogger<ErrorNotificationService> logger)
        {
            _emailSender = emailSender;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendErrorNotificationAsync(string errorMessage, string? stackTrace = null)
        {
            try
            {
                var serviceEmail = _configuration["Email:services"];
                if (string.IsNullOrEmpty(serviceEmail))
                {
                    _logger.LogWarning("Service email address not configured. Skipping error notification.");
                    return;
                }

                var subject = "Error Notification";
                var htmlMessage = $@"
                    <h2>Error Occurred</h2>
                    <p><strong>Time:</strong> {DateTime.UtcNow}</p>
                    <p><strong>Error Message:</strong> {errorMessage}</p>
                    {(string.IsNullOrEmpty(stackTrace) ? "" : $"<p><strong>Stack Trace:</strong><br/>{stackTrace}</p>")}
                ";

                await _emailSender.SendEmailAsync(serviceEmail, subject, htmlMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send error notification email");
            }
        }
    }
}
