namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public enum SendRejectReason
{
    BlockedByRecipient,    // recipient blocked the sender
    BlockedByYou,          // sender blocked recipient (policy forbids send)
    NotMember,             // sender is not a member of the conversation
    ConversationClosed,    // support thread closed
    RateLimited,           // per-user or per-conv throttling
    ContentTooLong,        // text length exceeded
    AttachmentNotReady,    // attachment placeholder not linked yet
    AttachmentFailed,      // upload/link failed (see ErrorCode)
    InvalidMime,           // disallowed MIME
    QuotaExceeded,         // storage/attachment quota
    ModerationRejected,    // content policy/moderation
    Unknown
}

public sealed record SendRejectedDto(
    int ConversationId,
    string SenderUserId,
    string ClientLocalId,      // client-generated id for idempotency & UI match
    SendRejectReason Reason,   // canonical reason
    bool Retryable,            // UI can show "Tap to retry" if true
    string? ErrorCode = null,  // short code e.g. "PresignedUrlExpired"
    string? ErrorMessage = null, // short human hint; no PII/stack traces
    DateTime? Until = null     // optional: when rate-limit/mute lifts
);