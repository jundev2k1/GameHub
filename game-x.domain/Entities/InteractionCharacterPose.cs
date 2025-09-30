namespace game_x.domain.Entities;

public sealed class InteractionCharacterPose : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public int CharacterId { get; private set; }
    public InteractionCharacter Character { get; private set; } = default!;
    public string DisplayName { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public int Priority { get; private set; }
    public int PoseId { get; private set; }
    public MediaFile Pose { get; private set; } = default!;

    public static InteractionCharacterPose Create(
        string displayName,
        string desc,
        string notes,
        int priority,
        MediaFile pose)
    {
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        return new InteractionCharacterPose
        {
            DisplayName = displayName.Trim(),
            Description = desc.Trim(),
            Notes = notes.Trim(),
            Priority = priority,
            Pose = pose,
        };
    }

    public void Update(string displayName, string desc, string notes, int priority, MediaFile pose)
    {
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        if (priority < 0)
            throw new ArgumentException("Priority must be greater than or equal 0.", nameof(priority));

        DisplayName = displayName.Trim();
        Description = desc.Trim();
        Notes = notes.Trim();
        Priority = priority;
        Pose = pose;
    }

    public void UpdatePose(MediaFile pose) => Pose = pose;
}
