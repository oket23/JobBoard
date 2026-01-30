namespace JobBoard.Notification.Worker.Interfaces;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string firstName);
    Task SendLoginEmailAsync(string toEmail, string firstName, DateTime loginTime);
    Task SendApplicationReceivedAsync(string toEmail, string firstName, string jobTitle);
    Task SendApplicationStatusChangedAsync(string toEmail, string firstName, string jobTitle, string newStatus);

}