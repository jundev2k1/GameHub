namespace game_x.share.ExternalApi.Atg.Dtos.Register;

public sealed class AtgRegisterRequest
{
    /// <summary>Username. Format: Alphanumeric (special characters are prohibited)
    /// Minimum length: 5, Maximum length: 64.</summary>
    public required string Username { get; set; }
    public string? Email { get; set; }
    /// <summary>User's date of birth in ISO-8601 format.</summary>
    public string? Birthday { get; set; }
    /// <summary>The default nationality code is Taiwan.</summary>
    public string? Country { get; set; }
    /// <summary>The user's name displayed in the game (if not set to display as username).</summary>
    public string? Fullname { get; set; }
    /// <summary>Game language: zh-cn, zh-tw, en, vn (default language settings for merchants).</summary>
    public string? Language { get; set; }
}