using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Attachments;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.infrastructure.Attachments;

public sealed class AttachmentBinder(
    IFileStorageService storage,
    IMediaFileRepo mediaRepo
) : IAttachmentBinder, IServices
{
    public async Task<(int?, AttachmentBindingStatus, string?)> BindAsync(
        AttachmentInputDto i, string addedByUserId, CancellationToken ct)
    {
        // lazy-finalize
        if (!string.IsNullOrWhiteSpace(i.TempObjectName))
        {
            var head = await storage.HeadObjectAsync(BucketName.Chat, ObjectName.Of(i.TempObjectName), ct);
            if (head is null) return (null, AttachmentBindingStatus.Failed, "object_missing");
        
            if (i.SizeBytes is null or <= 0) return (null, AttachmentBindingStatus.Failed, "size_invalid");
            if (string.IsNullOrWhiteSpace(i.MimeType)) return (null, AttachmentBindingStatus.Failed, "mime_required");
            if (string.IsNullOrWhiteSpace(i.FileName)) return (null, AttachmentBindingStatus.Failed, "filename_required");
        
            var media = MediaFile.Create(
                bucketName: BucketName.Chat,
                objectName: ObjectName.Of(i.TempObjectName!),
                fileName: i.FileName!,
                mimeType: MimeType.Of(i.MimeType!),
                sizeBytes: i.SizeBytes!.Value,
                metadata: "{}"
            );
            await mediaRepo.AddAsync(media, ct);
            return (media.Id, AttachmentBindingStatus.ReadyToLink, null);
        }

        return (null, AttachmentBindingStatus.Failed, "no_reference");
    }
}
