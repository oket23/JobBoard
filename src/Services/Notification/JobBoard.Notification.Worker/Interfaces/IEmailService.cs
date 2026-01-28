namespace JobBoard.Notification.Worker.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string firstName);
}