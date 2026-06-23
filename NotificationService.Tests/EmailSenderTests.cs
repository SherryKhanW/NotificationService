using NotificationService.Email;

namespace NotificationService.Tests;

public class EmailSenderTests
{
    private IEmailSender _emailSender;

    [SetUp]
    public void Setup()
    {
        _emailSender = new EmailSender();
    }

    [Test]
    public async Task SendOtpEmailAsync_ShouldReturnTrue()
    {
        var result = await _emailSender.SendOtpEmailAsync(
            "test@example.com",
            "123456");

        Assert.That(result, Is.True);
    }

    [Test]
    public async Task SendOtpEmailAsync_ShouldNotThrowException()
    {
        Assert.DoesNotThrowAsync(async () =>
            await _emailSender.SendOtpEmailAsync(
                "test@example.com",
                "123456"));
    }
}