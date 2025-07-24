namespace game_x.domain.Entities;

public sealed class StaffExtension : BaseEntity<int>
{
    public string StaffId { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;

    public AppUser? Staff { get; set; }
    public AppUser? Admin { get; private set; } = default!;

    public static StaffExtension Create(string createdBy, string? staffId = null)
    {
        var staffExtension = new StaffExtension
        {
            StaffId = staffId ?? string.Empty,
            CreatedBy = createdBy
        };

        return staffExtension;
    }
}
