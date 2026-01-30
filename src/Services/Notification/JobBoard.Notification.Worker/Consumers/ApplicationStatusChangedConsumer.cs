using JobBoard.Notification.Worker.Interfaces;
using JobBoard.Shared.Events.Applications;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobBoard.Notification.Worker.Consumers;

public class ApplicationStatusChangedConsumer : IConsumer<ApplicationStatusChangedEvent>
{
    private readonly ILogger<ApplicationStatusChangedConsumer> _logger;
    private readonly IEmailService _emailService;

    public ApplicationStatusChangedConsumer(ILogger<ApplicationStatusChangedConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }
    
    public async Task Consume(ConsumeContext<ApplicationStatusChangedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing status change for App ID {Id} -> {Status}", message.ApplicationId, message.NewStatus);

        try
        {
            await _emailService.SendApplicationStatusChangedAsync(message.CandidateEmail, message.CandidateFirstName, message.JobTitle, message.NewStatus);
            _logger.LogInformation("Email sent successfully to {Email}", message.CandidateEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send login email to {Email}", message.CandidateEmail);
        }
       
    }
}