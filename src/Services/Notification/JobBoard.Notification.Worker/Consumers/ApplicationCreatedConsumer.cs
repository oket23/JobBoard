using JobBoard.Notification.Worker.Interfaces;
using JobBoard.Shared.Events.Applications;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace JobBoard.Notification.Worker.Consumers;

public class ApplicationCreatedConsumer : IConsumer<JobApplicationCreatedEvent>
{
    private readonly ILogger<ApplicationCreatedConsumer> _logger;
    private readonly IEmailService _emailService;

    public ApplicationCreatedConsumer(ILogger<ApplicationCreatedConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<JobApplicationCreatedEvent> context)
    {
        var message = context.Message;
        _logger.LogInformation("Processing new application: {Job} by {Email}", message.JobTitle, message.CandidateEmail);

        try
        {
            await _emailService.SendApplicationReceivedAsync(message.CandidateEmail, message.CandidateFirstName, message.JobTitle);
            _logger.LogInformation("Email sent successfully to {Email}", message.CandidateEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send login email to {Email}", message.CandidateEmail);
        }
        
    }
}