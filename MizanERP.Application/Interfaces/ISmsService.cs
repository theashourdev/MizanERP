namespace MizanERP.Application.Interfaces;

public interface ISmsService
{
    Task SendOtpAsync(string phoneNumber, string otp);
    Task SendPasswordResetOtpAsync(string phoneNumber, string otp);
    Task SendAccountLockedNotificationAsync(string phoneNumber);
    Task SendAsync(string phoneNumber, string message);
}