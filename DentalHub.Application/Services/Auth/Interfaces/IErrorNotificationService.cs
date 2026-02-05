namespace DentalHub.Application.Services.Auth
{
    public interface IErrorNotificationService
    {
        Task SendErrorNotificationAsync(string errorMessage, string? stackTrace = null);
    }
}
