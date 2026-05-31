namespace game_x.application.Features.Attachments.Dtos;

public sealed record InitUploadResponse(
    string UploadUrl,
    BucketName BucketName,
    ObjectName ObjectName,
    IReadOnlyDictionary<string,string>? Headers // e.g. x-amz-acl, etc.
);