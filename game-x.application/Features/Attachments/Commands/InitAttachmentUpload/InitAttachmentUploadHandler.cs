using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Features.Attachments.Dtos;

namespace game_x.application.Features.Attachments.Commands.InitAttachmentUpload;

public sealed class InitAttachmentUploadHandler(
    IUserAccessor userAccessor,
    IFileStorageService fileStorage
) : ICommandHandler<InitAttachmentUploadCommand, InitUploadResponse>
{
    public async Task<InitUploadResponse> Handle(InitAttachmentUploadCommand cmd, CancellationToken ct)
    {
        var req = cmd.Request;

        // 1) Validate cơ bản
        if (string.IsNullOrWhiteSpace(req.FileName))
            throw new BadRequestException("file_name_required");
        if (req.SizeBytes <= 0)
            throw new BadRequestException("size_invalid");
        if (string.IsNullOrWhiteSpace(req.MimeType))
            throw new BadRequestException("mime_required");
        // (Optional) enforce allow-list mime types / max size tại đây
        
        var userId = userAccessor.GetUserId();

        var ext = Path.GetExtension(req.FileName);
        var newFileName = $"{Guid.NewGuid():N}{ext}";
        var objectName = ObjectName.Attachment(userId, newFileName);

        var bucket = BucketName.Chat;
        
        var ticket = await fileStorage.CreatePresignedPutAsync(
            bucket: bucket,
            objectName: objectName,
            mimeType: MimeType.Of(req.MimeType),
            sizeBytes: req.SizeBytes,
            expiry: TimeSpan.FromMinutes(10),
            ct: ct
        );
        
        return new InitUploadResponse(ticket.UploadUrl, bucket, objectName, ticket.Headers);
    }
}

// public sealed class InitAttachmentUploadHandler(
//     IUserAccessor userAccessor,
//     IFileStorageService fileStorage
// ) : ICommandHandler<InitAttachmentUploadCommand, InitUploadResponse>
// {
//     public async Task<InitUploadResponse> Handle(InitAttachmentUploadCommand cmd, CancellationToken ct)
//     {
//         var userId = userAccessor.GetUserId();
//         var uploadFile = cmd.FileUploads.FirstOrDefault()!;
//         var sizeBytes = Convert.ToInt32(uploadFile.Length);
//         var mimeType = MimeType.Of(uploadFile.ContentType);
//         var bucket = BucketName.Chat;
//         
//         // Validate
//         if (string.IsNullOrWhiteSpace(uploadFile.FileName))
//             throw new BadRequestException("file_name_required");
//         if (sizeBytes <= 0)
//             throw new BadRequestException("size_invalid");
//
//         var ext = Path.GetExtension(uploadFile.FileName);
//         var newFileName = $"{Guid.NewGuid():N}{ext}";
//         var objectName = ObjectName.Attachment(userId, newFileName);
//         
//         // 3) Tạo presigned PUT (FE sẽ PUT trực tiếp tới storage)
//         var ticket = await fileStorage.CreatePresignedPutAsync(
//             bucket: bucket,
//             objectName: objectName,
//             mimeType: mimeType,
//             sizeBytes: sizeBytes,
//             ttl: TimeSpan.FromMinutes(10),
//             ct: ct
//         );
//         
//         return new InitUploadResponse(ticket.UploadUrl, bucket, objectName, ticket.Headers);
//     }
// }