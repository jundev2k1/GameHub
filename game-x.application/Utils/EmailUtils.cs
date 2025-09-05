namespace game_x.application.Utils;

public static class EmailUtils
{
    public static string GenerateDummyEmail(string role)
    {
        return $"{role.ToLower()}-{Guid.NewGuid():N}@noemail.local";
    }
}
