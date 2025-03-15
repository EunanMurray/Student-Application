using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using StudentApplication.Settings;
using System.Text.Encodings.Web;

namespace StudentApplication.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlBody);
        Task<bool> SendVerificationEmailAsync(string to, string verificationLink);
        Task<bool> SendScholarshipOfferEmailAsync(string to, string name, string scholarshipLevel, string acceptanceLink);
        Task<bool> SendApplicationConfirmationEmailAsync(string to, string name);
        string GetLastError();
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly HtmlEncoder _htmlEncoder;
        private string _lastError = string.Empty;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger,
            HtmlEncoder htmlEncoder)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _htmlEncoder = htmlEncoder;
        }

        public string GetLastError() => _lastError;

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                // Debuggin galore as I think I busted the email setup (I did not just old repo version was published with old settings)
                _logger.LogInformation($"Attempting to send email to {to} with subject: {subject}");
                _logger.LogInformation($"SMTP Server: {_emailSettings.SmtpServer}, Port: {_emailSettings.SmtpPort}");

                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    Timeout = 30000
                };

                using var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.FromEmail, _emailSettings.FromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(to);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {to}");
                return true;
            }
            catch (Exception ex)
            {
                _lastError = $"Failed to send email: {ex.Message}";
                _logger.LogError(ex, _lastError);
                return false;
            }
        }

        public async Task<bool> SendVerificationEmailAsync(string to, string verificationLink)
        {
            try
            {
                var encodedLink = verificationLink != null ? _htmlEncoder.Encode(verificationLink) : "#";
                var subject = "Verify your email address";
                var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Welcome to the Sports Scholarship System</h2>
                    <p>Thank you for creating an account. Please verify your email address by clicking the button below:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{encodedLink}' 
                           style='background-color: #3498db; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                            Verify Email Address
                        </a>
                        <p>
                            Please check your spam folder. If you don't receive the email within a few minutes,
                        </p>
                    </div>
                    <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                    <p style='word-break: break-all;'>{encodedLink}</p>
                    <p style='color: #7f8c8d; font-size: 14px;'>If you did not create this account, you can safely ignore this email.</p>
                </div>";

                return await SendEmailAsync(to, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _lastError = $"Error preparing verification email: {ex.Message}";
                _logger.LogError(ex, _lastError);
                return false;
            }
        }

        public async Task<bool> SendApplicationConfirmationEmailAsync(string to, string name)
        {
            try
            {
                var encodedName = name != null ? _htmlEncoder.Encode(name) : "Applicant";
                var subject = "Application Submitted";
                var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Thank you {encodedName}!</h2>
                    <p>Your scholarship application has been submitted successfully.</p>
                    <p>You will receive an email once your application has been reviewed.</p>
                    <p style='color: #7f8c8d; font-size: 14px;'>If you have any questions, please contact the scholarship office.</p>
                </div>";

                return await SendEmailAsync(to, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _lastError = $"Error preparing application confirmation email: {ex.Message}";
                _logger.LogError(ex, _lastError);
                return false;
            }
        }

        public async Task<bool> SendScholarshipOfferEmailAsync(string to, string name, string scholarshipLevel, string acceptanceLink)
        {
            try
            {
                if (string.IsNullOrEmpty(acceptanceLink))
                {
                    _logger.LogWarning("Acceptance link is null or empty when sending scholarship email to {Email}", to);
                    _lastError = "Acceptance link is null or empty";
                    return false;
                }

                var encodedName = name != null ? _htmlEncoder.Encode(name) : "Applicant";
                var encodedLevel = scholarshipLevel != null ? _htmlEncoder.Encode(scholarshipLevel) : "Scholarship";
                var encodedLink = _htmlEncoder.Encode(acceptanceLink);

                var subject = $"Congratulations! {encodedLevel} Scholarship Offer";
                var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Congratulations {encodedName}!</h2>
                    <p>We are pleased to inform you that you have been offered a <strong>{encodedLevel} Scholarship</strong>.</p>
                    <p>Please review and accept your scholarship offer by clicking the button below:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{encodedLink}' 
                           style='background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                            Review and Accept Scholarship
                        </a>
                    </div>
                    <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                    <p style='word-break: break-all;'>{encodedLink}</p>
                    <p style='color: #7f8c8d; font-size: 14px;'>If you have any questions, please contact the scholarship office.</p>
                </div>";

                _logger.LogInformation("Sending scholarship offer email to {Email} for {Level} scholarship", to, scholarshipLevel);
                return await SendEmailAsync(to, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _lastError = $"Error preparing scholarship offer email: {ex.Message}";
                _logger.LogError(ex, _lastError);
                return false;
            }
        }
    }
}