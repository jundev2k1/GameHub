using System.Text.Json.Serialization;

namespace game_x.application.Contract.Infrastructure.Email;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);

    Task SendVerificationEmailAsync(string to, string code);

    Task SendResetPasswordEmailAsync(string to, string code);

    Task SendLiveStreamRemainderEmailAsync(string to, LivestreamSchedule schedule);

    Task SendLiveStreamCancellationEmailAsync(string to, LivestreamSchedule schedule);
}

public class EngageLabEmailRequest
{
    public string From { get; set; } = default!;
    public List<string> To { get; set; } = [];
    public EngageLabEmailBody Body { get; set; } = new();
    public Dictionary<string, string>? CustomArgs { get; set; }
    public string? RequestId { get; set; }
}

public class EngageLabEmailBody
{
    public List<string>? Cc { get; set; }
    public List<string>? Bcc { get; set; }
    public List<string>? ReplyTo { get; set; }
    public string Subject { get; set; } = default!;
    public EngageLabEmailContent Content { get; set; } = new();
    public Dictionary<string, List<string>>? Vars { get; set; }
    public Dictionary<string, string>? Headers { get; set; }
    public EngageLabEmailSettings? Settings { get; set; }
}

public class EngageLabEmailContent
{
    public string? Html { get; set; }
    public string? Text { get; set; }
    public string? PreviewText { get; set; }
}
public class EngageLabEmailSettings
{
    public int SendMode { get; set; } = 0;
    public bool ReturnEmailId { get; set; } = true;
    public bool Sandbox { get; set; } = false;
    public bool Notification { get; set; } = false;
    public bool OpenTracking { get; set; } = true;
    public bool ClickTracking { get; set; } = false;
    public bool UnsubscribeTracking { get; set; } = true;
}

public class EngageLabEmailResponse
{
    [JsonPropertyName("email_ids")]
    public List<string> EmailIds { get; set; } = default!;

    [JsonPropertyName("request_id")]
    public string RequestId { get; set; } = string.Empty;
}