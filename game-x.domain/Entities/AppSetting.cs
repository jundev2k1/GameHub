using System.Reflection;

namespace game_x.domain.Entities;

public sealed class AppSetting : BaseEntity<int>, IAuditable
{
    public string Key { get; private set; } = string.Empty;
    public string Value { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsEditable { get; private set; }

    public static AppSetting Create(string key, string value, string desc, bool isEdit = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(key));
        ArgumentException.ThrowIfNullOrWhiteSpace(key, nameof(value));

        var isExist = typeof(AppSettingConstant)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.Name.StartsWith("KEY_"))
            .Any(f => (string?)f.GetValue(null) == key);
        if (!isExist) throw new ArgumentException(key, nameof(key));

        return new AppSetting
        {
            Key = key,
            Value = value,
            Description = desc,
            IsEditable = isEdit,
        };
    }

    public void Update(string value, string desc)
    {
        Value = value;
        Description = desc;
    }
}
