using Microsoft.AspNetCore.Identity;

namespace game_x.domain.Entities;

public class UserRole : IdentityUserRole<string>, IEntity
{
    public Role Role { get; set; } = default!;
    public User User { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
