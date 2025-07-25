using Microsoft.AspNetCore.Identity;

namespace game_x.domain.Entities;

public class Role : IdentityRole, IEntity<string>
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

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
