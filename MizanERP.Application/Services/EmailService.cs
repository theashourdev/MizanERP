using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MizanERP.Application.Interfaces;
using System.Net;
using MailKitSmtpClient = MailKit.Net.Smtp;
using SystemSmtpClient = System.Net.Mail;
namespace MizanERP.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _appName;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        var emailSettings = configuration.GetSection("EmailSettings");
        _fromEmail = emailSettings["FromEmail"] ?? "noreply@mizanerp.com";
        _fromName = emailSettings["FromName"] ?? "MizanERP";
        _appName = configuration["AppSettings:AppName"] ?? "MizanERP";
    }

    public async Task SendEmailVerificationAsync(string toEmail, string userName, string verificationLink)
    {
        var subject = $"Verify Your {_appName} Email";
        var body = GetEmailTemplate(
            title: "Verify Your Email",
            greeting: $"Hello {userName},",
            content: $"Thank you for registering. Please verify your email address by clicking the button below.",
            buttonText: "Verify Email",
            buttonUrl: verificationLink,
            footer: "This link expires in 48 hours. If you didn't register, please ignore this email."
        );

        await SendAsync(toEmail, subject, body);
    }

    public async Task SendPasswordResetAsync(string toEmail, string userName, string resetLink)
    {
        var subject = $"Reset Your {_appName} Password";
        var body = GetEmailTemplate(
            title: "Password Reset Request",
            greeting: $"Hello {userName},",
            content: "We received a request to reset your password. Click the button below to create a new password.",
            buttonText: "Reset Password",
            buttonUrl: resetLink,
            footer: "This link expires in 24 hours. If you didn't request this, please ignore this email and your password will remain unchanged."
        );

        await SendAsync(toEmail, subject, body);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName, string tempPassword)
    {
        var clientUrl = _configuration["AppSettings:ClientUrl"];
        var subject = $"Welcome to {_appName}!";
        var body = GetEmailTemplate(
            title: $"Welcome to {_appName}",
            greeting: $"Hello {userName},",
            content: $"Your account has been created successfully.<br><br>" +
                     $"<strong>Email:</strong> {toEmail}<br>" +
                     $"<strong>Temporary Password:</strong> <code>{tempPassword}</code><br><br>" +
                     $"Please login and change your password immediately.",
            buttonText: "Login Now",
            buttonUrl: clientUrl ?? "#",
            footer: "For security reasons, please change your password after your first login."
        );

        await SendAsync(toEmail, subject, body);
    }

    public async Task SendPasswordChangedNotificationAsync(string toEmail, string userName)
    {
        var subject = $"{_appName} — Password Changed";
        var body = GetEmailTemplate(
            title: "Password Changed",
            greeting: $"Hello {userName},",
            content: "Your password was successfully changed. If you did not make this change, please contact support immediately.",
            buttonText: null,
            buttonUrl: null,
            footer: $"This is an automated security notification from {_appName}."
        );

        await SendAsync(toEmail, subject, body);
    }

    public async Task SendAccountLockedNotificationAsync(string toEmail, string userName)
    {
        var subject = $"{_appName} — Account Locked";
        var body = GetEmailTemplate(
            title: "Account Locked",
            greeting: $"Hello {userName},",
            content: "Your account has been temporarily locked due to multiple failed login attempts. Please use the 'Forgot Password' option to regain access.",
            buttonText: null,
            buttonUrl: null,
            footer: "If this wasn't you, please contact support immediately."
        );

        await SendAsync(toEmail, subject, body);
    }

    public async Task SendAsyncDefault(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var host = emailSettings["SmtpHost"] ?? throw new InvalidOperationException("SMTP Host not configured.");
            var port = emailSettings.GetValue<int>("SmtpPort", 587);
            var username = emailSettings["SmtpUsername"];
            var password = emailSettings["SmtpPassword"];
            var enableSsl = emailSettings.GetValue<bool>("EnableSsl", true);

            using var client = new SystemSmtpClient.SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                Credentials = new NetworkCredential(username, password)
            };

            var message = new SystemSmtpClient.MailMessage
            {
                From = new SystemSmtpClient.MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            await client.SendMailAsync(message);
            _logger.LogInformation("Email sent to {Email} — Subject: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        try
        {
            var emailSettings = _configuration.GetSection("EmailSettings");

            var host = emailSettings["SmtpHost"];
            var port = emailSettings.GetValue<int>("SmtpPort");
            var username = emailSettings["SmtpUsername"];
            var password = emailSettings["SmtpPassword"];
            var enableSsl = emailSettings.GetValue<bool>("EnableSsl");

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(_fromName, _fromEmail));

            email.To.Add(MailboxAddress.Parse(toEmail));

            email.Subject = subject;

            email.Body = new BodyBuilder
            {
                HtmlBody = htmlBody
            }.ToMessageBody();

            using var smtp = new MailKitSmtpClient.SmtpClient();

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(username, password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);

            _logger.LogInformation(
                "Email sent to {Email}",
                toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }
    // ─── HTML Email Template ──────────────────────────────────────────────────
    public string GetEmailTemplate(
        string title,
        string greeting,
        string content,
        string? buttonText,
        string? buttonUrl,
        string footer)
    {
        var buttonHtml = (!string.IsNullOrEmpty(buttonText) && !string.IsNullOrEmpty(buttonUrl))
            ? $@"<tr>
                    <td align=""center"" style=""padding: 30px 0;"">
                        <a href=""{buttonUrl}""
                           style=""background-color:#2563EB;color:#ffffff;padding:14px 32px;
                                  text-decoration:none;border-radius:6px;font-size:16px;
                                  font-weight:bold;display:inline-block;"">
                            {buttonText}
                        </a>
                    </td>
                 </tr>"
            : string.Empty;

        return $@"<!DOCTYPE html>
<html>
<head><meta charset=""UTF-8""><meta name=""viewport"" content=""width=device-width,initial-scale=1""></head>
<body style=""margin:0;padding:0;background-color:#f4f4f4;font-family:Arial,sans-serif;"">
  <table width=""100%"" cellpadding=""0"" cellspacing=""0"">
    <tr>
      <td align=""center"" style=""padding:40px 10px;"">
        <table width=""600"" cellpadding=""0"" cellspacing=""0""
               style=""background-color:#ffffff;border-radius:8px;overflow:hidden;
                      box-shadow:0 2px 8px rgba(0,0,0,0.1);"">

          <!-- Header -->
          <tr>
            <td style=""background-color:#1e3a5f;padding:30px;text-align:center;"">
              <h1 style=""color:#ffffff;margin:0;font-size:24px;"">{_appName}</h1>
            </td>
          </tr>

          <!-- Body -->
          <tr>
            <td style=""padding:40px 40px 10px;"">
              <h2 style=""color:#1e3a5f;margin:0 0 20px;"">{title}</h2>
              <p style=""color:#444;font-size:16px;line-height:1.6;margin:0 0 10px;"">{greeting}</p>
              <p style=""color:#444;font-size:16px;line-height:1.6;margin:0;"">{content}</p>
            </td>
          </tr>

          <!-- Button -->
          {buttonHtml}

          <!-- Footer -->
          <tr>
            <td style=""background-color:#f9f9f9;padding:20px 40px;border-top:1px solid #eee;"">
              <p style=""color:#999;font-size:13px;line-height:1.5;margin:0;text-align:center;"">
                {footer}
              </p>
              <p style=""color:#bbb;font-size:12px;margin:10px 0 0;text-align:center;"">
                © {DateTime.UtcNow.Year} {_appName}. All rights reserved.
              </p>
            </td>
          </tr>

        </table>
      </td>
    </tr>
  </table>
</body>
</html>";
    }
}
