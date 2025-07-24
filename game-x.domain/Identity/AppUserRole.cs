using Microsoft.AspNetCore.Identity;

namespace game_x.domain.Identity;

public class AppUserRole : IdentityUserRole<string>
{
    public virtual IdentityRole Role { get; set; } = default!;
    public virtual AppUser User { get; set; } = default!;
}