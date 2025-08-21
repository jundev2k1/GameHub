namespace game_x.share.Helper;

public static class UserAgentHelper
{
    public static string GetDeviceKey(string userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return "Unknown";

        var ua = userAgent.ToLowerInvariant();

        // OS detection
        var os = ua switch
        {
            string s when s.Contains("windows nt") => "Windows",
            string s when s.Contains("mac os") => "MacOS",
            string s when s.Contains("android") => "Android",
            string s when s.Contains("iphone") => "iOS",
            string s when s.Contains("ipad") => "iOS",
            string s when s.Contains("linux") => "Linux",
            _ => "UnknownOS"
        };

        // Browser detection
        var browser = ua switch
        {
            string s when s.Contains("edg") => "Edge",
            string s when s.Contains("chrome") => "Chrome",
            string s when s.Contains("safari") && !s.Contains("chrome") => "Safari",
            string s when s.Contains("firefox") => "Firefox",
            string s when s.Contains("opr") || s.Contains("opera") => "Opera",
            _ => "UnknownBrowser"
        };

        return $"{os}_{browser}";
    }
}
