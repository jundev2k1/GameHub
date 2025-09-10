namespace game_x.application.Features.Attachments.Dtos;

public sealed record FinalizeUploadRequest(string ObjectName, string? FileName, string? MimeType, int? SizeBytes);