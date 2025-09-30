namespace game_x.domain.Entities;

public sealed class InteractionCharacter : BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; private set; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;

    public int DefaultPoseId { get; private set; }
    public MediaFile DefaultPose { get; private set; } = default!;

    public ICollection<InteractionCharacterPose> Poses { get; private set; } = [];

    public static InteractionCharacter Create(
        string name,
        string desc,
        string notes)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return new InteractionCharacter
        {
            Name = name.Trim(),
            Description = desc.Trim(),
            Notes = notes.Trim(),
        };
    }

    public void SetDefaultPose(MediaFile defaultPose)
    {
        DefaultPose = defaultPose;
    }
}
