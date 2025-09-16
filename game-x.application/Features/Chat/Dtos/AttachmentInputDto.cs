namespace game_x.application.Features.Chat.Dtos;

public sealed record AttachmentInputDto(
    string ClientId,
    int SortOrder,
    // Lazy-finalize fields:
    string? TempObjectName = null,
    string? FileName = null,
    string? MimeType = null,
    int? SizeBytes = null
);