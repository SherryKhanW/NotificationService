using NotificationService.Email;
using NotificationService.Grpc.Contracts;
using NotificationService.Models;
using NotificationService.Repositories;
using ProtoBuf.Grpc;

namespace NotificationService.Grpc.Services;

public class NotificationGrpcService : INotificationGrpcService
{
    private readonly IEmailSender _emailSender;
    private readonly IRepository<EmailLetter> _emailLetterRepository;

    public NotificationGrpcService(
        IEmailSender emailSender,
        IRepository<EmailLetter> emailLetterRepository)
    {
        _emailSender = emailSender;
        _emailLetterRepository = emailLetterRepository;
    }

    public async Task<SendOtpEmailResponse> SendOtpEmailAsync(
        SendOtpEmailRequest request,
        CallContext context = default)
    {
        var letter = new EmailLetter
        {
            ToEmail = request.Email,
            Subject = "Your OTP Code",
            Body = $"Your OTP code is: {request.Otp}",
            Status = EmailStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _emailLetterRepository.CreateAsync(letter);

        try
        {
            await _emailSender.SendOtpEmailAsync(request.Email, request.Otp);

            letter.Status = EmailStatus.Sent;
            letter.SentAt = DateTime.UtcNow;

            await _emailLetterRepository.UpdateAsync(letter);

            return new SendOtpEmailResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            letter.Status = EmailStatus.Failed;
            letter.ErrorMessage = ex.Message;

            await _emailLetterRepository.UpdateAsync(letter);

            return new SendOtpEmailResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
}
