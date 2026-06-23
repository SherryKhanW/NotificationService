namespace NotificationService.Email;

public interface IEmailSender
{
    Task<bool> SendOtpEmailAsync(string email, string otp);
}