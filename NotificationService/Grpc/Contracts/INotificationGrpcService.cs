using System.ServiceModel;

namespace NotificationService.Grpc.Contracts;

[ServiceContract]
public interface INotificationGrpcService
{
    [OperationContract]
    Task<SendOtpEmailResponse> SendOtpEmailAsync(
        SendOtpEmailRequest request);
}