using MassTransit;
using JobBoard.Notification.Worker.Interfaces;
using JobBoard.Shared.Events.User;
using Microsoft.Extensions.Logging;

namespace JobBoard.Notification.Worker.Consumers;

public class UserLoggedInConsumer : IConsumer<UserLoginEvent>
{
    private readonly IEmailService _emailService;
    private readonly ILogger<UserLoggedInConsumer> _logger;

    public UserLoggedInConsumer(IEmailService emailService, ILogger<UserLoggedInConsumer> logger)
    {
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserLoginEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing login notification for {Email} at {Time}", message.Email, message.Timestamp);

        try 
        {
            await _emailService.SendLoginEmailAsync(message.Email, message.FirstName, message.Timestamp);
            _logger.LogInformation("Email sent successfully to {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send login email to {Email}", message.Email);
        }
    }
}