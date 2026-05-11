using System.ComponentModel.DataAnnotations;
using game_x.domain.Enum.Missions;

namespace game_x.domain.Entities.Missions;

/// <summary>Generic user behavior log. Used for mission progression triggers.</summary>
public sealed class UserEvent : BaseEntity<int>
{
    #region Identities
    public Guid PublicId { get; private set; } = Guid.CreateVersion7();
    
    [MaxLength(36)]
    public string UserId { get; private set; } = string.Empty;
    #endregion

    #region Properties
    public UserEventType Type { get; private set; }

    public decimal? Value { get; private set; }

    public UserEventRefType? RefType { get; private set; }

    public int? RefId { get; private set; }

    [MaxLength(4096)]
    public string? Metadata { get; private set; }
    #endregion

    #region Relationships
    public User? User { get; private set; }
    #endregion

    #region Initializations
    private UserEvent() { }

    public static UserEvent Create(
        string userId,
        UserEventType type,
        decimal? value = null,
        UserEventRefType? refType = null,
        int? refId = null,
        string? metadata = null)
    {
        return new()
        {
            UserId = userId,
            Type = type,
            Value = value,
            RefType = refType,
            RefId = refId,
            Metadata = metadata
        };
    }
    #endregion
}