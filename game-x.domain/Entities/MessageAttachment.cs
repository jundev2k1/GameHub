using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class MessageAttachment : BaseEntity<int>, IAuditable
{
    public int MessageId { get; set; }
    public Message Message { get; set; } = null!;

    public int? MediaFileId { get; set; }
    public MediaFile? MediaFile { get; set; }

    public int SortOrder { get; set; }

    [MaxLength(64)] 
    public string? AddedByActorId { get; set; }
    [MaxLength(64)] 
    public string? AddedByUserId { get; set; }
    public User? AddedByUser { get; set; }
    
    public AttachmentBindingStatus BindingStatus { get; set; } = AttachmentBindingStatus.Pending;
    [MaxLength(64)]
    public string? Error { get; set; } // Machine-readable error code for the last failed attachment operation.
    
    public static MessageAttachment Create(
        Message msg,
        MediaFile? mediaFile,
        string addedByActorId,
        AttachmentBindingStatus bindingStatus,
        int sortOrder,
        string? addedByUserId = null,
        string? err = null
    )
    {
        var msgAttachment = new MessageAttachment
        {
            Message = msg,
            AddedByActorId = addedByActorId,
            AddedByUserId = addedByUserId,
            MediaFile = mediaFile,
            Error = err,
            BindingStatus = bindingStatus,
            SortOrder = sortOrder
        };
        return msgAttachment;
    }
}