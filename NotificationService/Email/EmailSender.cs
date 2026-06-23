using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using NotificationService.Configuration;

namespace NotificationService.Email;

public class EmailSender : IEmailSender
{
    private readonly SmtpOptions _smtpOptions;

    public EmailSender(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task<bool> SendOtpEmailAsync(string email, string otp)
    {
        var message = new MimeMessage();

        message.From.Add(MailboxAddress.Parse(_smtpOptions.From));
        message.To.Add(MailboxAddress.Parse(email));
        message.Subject = "Your OTP Code";

        message.Body = new TextPart("plain")
        {
            Text = $"Your OTP code is: {otp}"
        };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _smtpOptions.Host,
            _smtpOptions.Port,
            SecureSocketOptions.StartTlsWhenAvailable);

        await client.AuthenticateAsync(
            _smtpOptions.Username,
            _smtpOptions.Password);

        await client.SendAsync(message);

        await client.DisconnectAsync(true);

        return true;
    }
}