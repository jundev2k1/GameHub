using game_x.application.Common.Abstractions;
using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Files;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;
using game_x.infrastructure.Extensions;
using game_x.share.Extensions;
using game_x.share.Helper;

namespace game_x.infrastructure.Identity;

public class MessageService(
    IMessageRepo messageRepo,
    IMessageAttachmentRepo messageAttachmentRepo,
    IFileStorageService fileStorage
    ): IMessageService, IServices
{
    public async Task<CursorResult<ListMessageDto>> GetByCursorAsync(Guid convId, int limit, string? cursor, CancellationToken ct)
    {
        var query = await messageRepo.GetByCursorAsync(convId, limit, cursor, ct);
        var fp = CursorHelper.ComputeFp($"conv:{convId}");

        var entityResult = await SeekCursorBuilder<MessageDto>
            .For(query)
            .Keys(m => m.SentAt, m => m.Id)
            .Sort(desc1: true, desc2: false)
            .FromCursor(cursor, fp)
            .Limit(limit)
            .ExecuteAsync(m => m, ct);

        var dtoItems = await Task.WhenAll(
            entityResult.Items.Select(m => GetMessageDtoAsync(m, ct))
        );

        return entityResult.Transform(dtoItems.Reverse()); 
    }
    
    public async Task<ListMessageDto> GetMessageDtoAsync(MessageDto msg, CancellationToken ct)
    {
        var attachmentTasks = msg.Attachments
            .Select(async item =>
            {
                string? url = null;
                if (item.BucketName is not null && item.ObjectName is not null)
                {
                    url = await fileStorage.GenerateDownloadUrlAsync(
                        bucketName: item.BucketName,
                        objectName: item.ObjectName,
                        expiry: TimeSpan.FromMinutes(60),
                        ct: ct);
                }
                
                return new ListMessageAttachmentDto
                (
                    SortOrder: item.SortOrder,
                    BindingStatus: item.BindingStatus.ToString().ToCamelCase(),
                    FileName: item.FileName,
                    Url: url
                );
            });
        
        var attachmentsDto = await Task.WhenAll(attachmentTasks);
        
        return msg.Adapt<ListMessageDto>() with { Attachments = attachmentsDto };
    }
    
    public async Task CreateMessageAttachmentsAsync(
        Message msg,
        IReadOnlyList<FileUpload>? attachments, 
        CancellationToken ct)
    {
        if (attachments is not { Count: > 0 }) return;

        var mediaFiles = await UploadFiles(msg.SenderActorId, attachments, ct);
        foreach (var (file, index) in mediaFiles.Select((a, i) => (a, i)))
        {
            var msgAttachment = MessageAttachment.Create(
                msg: msg,
                sortOrder: index,
                mediaFile: file,
                addedByUserId: msg.SenderUserId,
                addedByActorId: msg.SenderActorId,
                bindingStatus: AttachmentBindingStatus.Linked);

            await messageAttachmentRepo.AddAsync(msgAttachment, ct);
        }
    }

    private async Task<IList<MediaFile>> UploadFiles(string userId, IReadOnlyList<FileUpload> files, CancellationToken ct)
    {
        IList<MediaFile> mediaFiles = [];
        foreach (var file in files)
        {
            var newFileName = Guid.NewGuid() + file.Extension;
            var objectName = ObjectName.Attachment(userId, newFileName);
            await fileStorage.UploadFileAsync(file.Content, BucketName.User, objectName, MimeType.Of(file.ContentType), ct);
            mediaFiles.Add(CreateMediaFile(file, objectName));
        }

        return mediaFiles;
    }
    
    private static MediaFile CreateMediaFile(FileUpload file, ObjectName objectName)
    {
        return MediaFile.Create(
            bucketName: BucketName.User,
            objectName: objectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
    }
}