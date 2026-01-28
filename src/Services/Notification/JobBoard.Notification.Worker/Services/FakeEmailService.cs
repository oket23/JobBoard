using JobBoard.Notification.Worker.Interfaces;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;

namespace JobBoard.Notification.Worker.Services;

public class FakeEmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<FakeEmailService> _logger;
    
    public FakeEmailService(IConfiguration config, ILogger<FakeEmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string firstName)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(
            _config["EmailSettings:SenderName"] ?? "JobBoard", 
            _config["EmailSettings:SenderEmail"] ?? "no-reply@jobboard.com"));
        
        if (!MailboxAddress.TryParse(toEmail, out var mailbox))
        {
            _logger.LogError("SKIPPING EMAIL: The address '{Email}' has an invalid format.", toEmail);
            return; 
        }
        
        message.To.Add(new MailboxAddress(firstName, toEmail));
        message.Subject = "Welcome to the team!";

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #007bff;'>Hello, {firstName}!</h2>
                    <p>Welcome to <strong>JobBoard</strong>. We are glad to see you in our community.</p>
                    <hr/>
                    <small>If you didn't register on our site, please ignore this email.</small>
                </div>"
        };

        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        
        var host = _config["EmailSettings:SmtpHost"] ?? "mailhog";
        var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "1025");
        
        await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.Auto);
        
        var user = _config["EmailSettings:Username"];
        var pass = _config["EmailSettings:Password"];
        
        if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
        {
            await client.AuthenticateAsync(user, pass);
        }

        await client.SendAsync(message);
        _logger.LogInformation("Email successfully prepared for {Email}", toEmail);
        await client.DisconnectAsync(true);
    }
}