using JobBoard.Notification.Worker.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace JobBoard.Notification.Worker.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }
    
    public async Task SendWelcomeEmailAsync(string toEmail, string firstName)
    {
        var subject = "Welcome to JobBoard! 🚀";
        var body = $@"
            <h2>Hi {firstName}, welcome aboard!</h2>
            <p>We are thrilled to have you in our community.</p>
            <p>You can now search for jobs and apply in one click.</p>
            <hr/>
            <small>JobBoard Team</small>";

        await SendEmailBaseAsync(toEmail, firstName, subject, body);
    }
    
    public async Task SendLoginEmailAsync(string toEmail, string firstName, DateTime loginTime)
    {
        var subject = "New Login Detected 🔐";
        var body = $@"
            <h3>Hello {firstName},</h3>
            <p>We noticed a new login to your account.</p>
            <p><strong>Time:</strong> {loginTime:yyyy-MM-dd HH:mm:ss} UTC</p>
            <p>If this wasn't you, please contact support immediately.</p>";

        await SendEmailBaseAsync(toEmail, firstName, subject, body);
    }
    
    public async Task SendApplicationReceivedAsync(string toEmail, string firstName, string jobTitle)
    {
        var subject = $"Application Received: {jobTitle} 📄";
        var body = $@"
            <h3>Good news, {firstName}!</h3>
            <p>Your application for the position <strong>{jobTitle}</strong> has been successfully received.</p>
            <p>The employer will review it shortly.</p>
            <br/>
            <a href='#'>View My Applications</a>";

        await SendEmailBaseAsync(toEmail, firstName, subject, body);
    }
    
    public async Task SendApplicationStatusChangedAsync(string toEmail, string firstName, string jobTitle, string newStatus)
    {
        var subject = $"Update on your application for {jobTitle}";
        
        string statusColor = newStatus.ToLower() switch
        {
            "accepted" => "#28a745", 
            "rejected" => "#dc3545", 
            _ => "#6c757d"           
        };

        var body = $@"
            <h3>Hello {firstName},</h3>
            <p>The status of your application for <strong>{jobTitle}</strong> has been updated.</p>
            <p>New Status: <b style='color:{statusColor}; font-size: 1.2em;'>{newStatus.ToUpper()}</b></p>
            <p>Log in to your dashboard for more details.</p>";

        await SendEmailBaseAsync(toEmail, firstName, subject, body);
    }
    
    private async Task SendEmailBaseAsync(string toEmail, string toName, string subject, string htmlContent)
    {
        try
        {
            if (!MailboxAddress.TryParse(toEmail, out var mailboxAddress))
            {
                _logger.LogError("Invalid email format: {Email}", toEmail);
                return;
            }
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["EmailSettings:SenderName"] ?? "JobBoard Robot",
                _config["EmailSettings:SenderEmail"] ?? "no-reply@jobboard.com"
            ));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            
            var finalHtml = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; border: 1px solid #eee; border-radius: 5px; max-width: 600px;'>
                    {htmlContent}
                    <div style='margin-top: 20px; font-size: 12px; color: #999;'>
                        <hr/>
                        © {DateTime.Now.Year} JobBoard Inc. All rights reserved.
                    </div>
                </div>";

            message.Body = new BodyBuilder { HtmlBody = finalHtml }.ToMessageBody();
            
            using var client = new SmtpClient();
            
            var host = _config["EmailSettings:SmtpHost"] ?? "jobboard-mailhog";
            var port = int.Parse(_config["EmailSettings:SmtpPort"] ?? "1025");

            await client.ConnectAsync(host, port, SecureSocketOptions.Auto);
            
            var user = _config["EmailSettings:Username"];
            var pass = _config["EmailSettings:Password"];
            if (!string.IsNullOrEmpty(user) && !string.IsNullOrEmpty(pass))
            {
                await client.AuthenticateAsync(user, pass);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {Email} | Subject: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
        }
    }
}