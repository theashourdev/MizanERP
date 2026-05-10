namespace MizanERP.Application.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string toEmail, string userName, string verificationLink);
    Task SendPasswordResetAsync(string toEmail, string userName, string resetLink);
    Task SendWelcomeEmailAsync(string toEmail, string userName, string tempPassword);
    Task SendPasswordChangedNotificationAsync(string toEmail, string userName);
    Task SendAccountLockedNotificationAsync(string toEmail, string userName);
    Task SendAsync(string toEmail, string subject, string htmlBody);
    Task SendEmailOtpAsync(string toEmail, string userName, string otp, DateTime expiry);

    string GetEmailTemplate(
        string title,
        string greeting,
        string content,
        string? buttonText,
        string? buttonUrl,
        string footer);
}