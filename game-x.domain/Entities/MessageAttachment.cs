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
    public string AddedByUserId { get; set; } = null!;
    public User AddedByUser { get; set; } = null!;
    
    public AttachmentBindingStatus BindingStatus { get; set; } = AttachmentBindingStatus.Pending;
    [MaxLength(64)]
    public string? Error { get; set; } // Machine-readable error code for the last failed attachment operation.
}