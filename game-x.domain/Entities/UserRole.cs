using Microsoft.AspNetCore.Identity;

namespace game_x.domain.Entities;

public class UserRole : IdentityUserRole<string>
{
    public virtual IdentityRole Role { get; set; } = default!;
    public virtual User User { get; set; } = default!;
}
