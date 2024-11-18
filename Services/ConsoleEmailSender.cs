using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        _logger.LogInformation($"Sending email to {email}: Subject: {subject}, Message: {htmlMessage}");
        return Task.CompletedTask;
    }
}
