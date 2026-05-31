using Microsoft.AspNetCore.Identity;

namespace game_x.domain.Entities;

public class Role : IdentityRole, IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];

    public static Role Create(string name, string? id = null)
    {
        var entity = new Role()
        {
            Name = name,
            NormalizedName = name.ToUpper(),
        };
        entity.Id = id ?? entity.Id;
        return entity;
    }
}