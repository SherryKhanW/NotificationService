namespace NotificationService.Models;

public class EmailLetter
{
    public int Id { get; set; }

    public string ToEmail { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public EmailStatus Status { get; set; } = EmailStatus.Pending;

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SentAt { get; set; }
}

public enum EmailStatus
{
    Pending = 0,
    Sent = 1,
    Failed = 2
}