namespace game_x.domain.Enum;

public enum AttachmentBindingStatus
{
    Pending = 1,     // Created with a message, no MediaFileId yet
    Uploading = 2,   // Client is uploading the file
    ReadyToLink = 3, // Upload complete, pending finalize and MediaFileId assignment
    Linked = 4,      // MediaFileId assigned (viewable)
    Failed = 5,      // Failed (stores Error) – allows retry
    Revoked = 6
}