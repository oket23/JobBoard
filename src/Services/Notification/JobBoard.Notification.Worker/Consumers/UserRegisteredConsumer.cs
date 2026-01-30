using JobBoard.Notification.Worker.Interfaces;
using JobBoard.Shared.Events.User;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobBoard.Notification.Worker.Consumers;

public class UserRegisteredConsumer : IConsumer<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredConsumer> _logger;
    private readonly IEmailService _emailService;
    
    public UserRegisteredConsumer(ILogger<UserRegisteredConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UserRegisteredEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Sending welcome email to: {Email} for user {FirstName}", message.Email, message.FirstName);

        try
        {
            await _emailService.SendWelcomeEmailAsync(message.Email, message.FirstName);
            _logger.LogInformation("Email sent successfully to {Email}", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send login email to {Email}", message.Email);
        }
        
    }
}