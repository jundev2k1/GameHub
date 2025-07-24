using Microsoft.AspNetCore.Identity;

namespace game_x.domain.Identity;

public class AppUser : IdentityUser, IBaseEntity<string>, IAuditable
{
    public ICollection<AppUserRole> UserRoles { get; set; } = [];
    public UserPassport? Passport { get; set; }
    public StaffUser? StaffUser { get; set; }
    public ICollection<StaffUser>? StaffUsers { get; set; }
    public bool IsDeleted { get; set; }
    public bool IsNew { get; set; } = true;
    public string? CountryCode { get; set; }
    public StaffExtension? StaffExtension { get; set; }
    public ICollection<StaffExtension>? StaffExtensions { get; set; }
    public AppUserStatus Status { get; set; } = AppUserStatus.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public void UpdateStatus(AppUserStatus status)
    {
        Status = status;
    }

    public (bool Result, System.Enum? ErrorCode) CheckValidUser()
    {
        if (Status == AppUserStatus.Inactive)
            return (false, MessageCode.User.UserInvalid);
        if (IsDeleted)
            return (false, MessageCode.User.UserDisabled);

        return (true, null);
    }
}
