using System.Collections;
using System.Reflection;

namespace game_x.domain.Identity;

public sealed class AppRole : IReadOnlyCollection<string>
{
    private readonly HashSet<string> _roles;
    private AppRole(IEnumerable<string> roles)
    {
        var roleList = typeof(AppRoles).GetFields(BindingFlags.Public | BindingFlags.Static)
            .Select(f => f.GetValue(null).ToStringOrEmpty());
        var inputs = roles.Select(r => r.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (inputs.Any(r => !roleList.Contains(r)))
            throw new ArgumentException("Invalid role(s) provided.", nameof(roles));

        _roles = inputs;
    }

    public static AppRole Of(params IEnumerable<string> roles) => new(roles);
    public static string ForAuthorize(params IEnumerable<string> roles) => Of(roles).ToString();

    public  bool Has(string role) => _roles.Contains(role.Trim(), StringComparer.OrdinalIgnoreCase);

    public bool HasAny(params string[] roles) => roles.Any(Has);

    public bool HasAll(params string[] roles) => roles.All(Has);

    public override string ToString() => _roles.ToArray().JoinToString(",");

    public IEnumerator<string> GetEnumerator() => _roles.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _roles.Count;
    public string[] Items => _roles.ToArray();
    public bool IsRoot => Has(AppRoles.Root);
    public bool IsAdmin => Has(AppRoles.Admin);
    public bool IsStaff => Has(AppRoles.Staff);
    public bool IsUser => Has(AppRoles.User);
}
