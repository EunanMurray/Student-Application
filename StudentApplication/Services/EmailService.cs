using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using StudentApplication.Settings;
using System.Text.Encodings.Web;

namespace StudentApplication.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task SendVerificationEmailAsync(string to, string verificationLink);
        Task SendScholarshipOfferEmailAsync(string to, string name, string scholarshipLevel, string acceptanceLink);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;
        private readonly HtmlEncoder _htmlEncoder;

        public EmailService(
            IOptions<EmailSettings> emailSettings,
            ILogger<EmailService> logger,
            HtmlEncoder htmlEncoder)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _htmlEncoder = htmlEncoder;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        public async Task SendVerificationEmailAsync(string to, string verificationLink)
        {
            var encodedLink = _htmlEncoder.Encode(verificationLink);
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
                    </div>
                    <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                    <p style='word-break: break-all;'>{encodedLink}</p>
                    <p style='color: #7f8c8d; font-size: 14px;'>If you did not create this account, you can safely ignore this email.</p>
                </div>";

            await SendEmailAsync(to, subject, htmlBody);
        }

        public async Task SendScholarshipOfferEmailAsync(string to, string name, string scholarshipLevel, string acceptanceLink)
        {
            var encodedLink = _htmlEncoder.Encode(acceptanceLink);
            var subject = $"Congratulations! {scholarshipLevel} Scholarship Offer";
            var htmlBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2 style='color: #2c3e50;'>Congratulations {_htmlEncoder.Encode(name)}!</h2>
                    <p>We are pleased to inform you that you have been offered a <strong>{_htmlEncoder.Encode(scholarshipLevel)} Scholarship</strong>.</p>
                    <p>Please review and accept your scholarship offer by clicking the button below:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{encodedLink}' 
                           style='background-color: #27ae60; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>
                            Review and Accept Scholarship
                        </a>
                    </div>
                    <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                    <p style='word-break: break-all;'>{encodedLink}</p>
                    <p>This offer will expire in 14 days.</p>
                    <p style='color: #7f8c8d; font-size: 14px;'>If you have any questions, please contact the scholarship office.</p>
                </div>";

            await SendEmailAsync(to, subject, htmlBody);
        }
    }
}