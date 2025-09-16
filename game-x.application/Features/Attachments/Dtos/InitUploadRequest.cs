namespace game_x.application.Features.Attachments.Dtos;

public sealed record InitUploadRequest(string FileName, string MimeType, int SizeBytes);