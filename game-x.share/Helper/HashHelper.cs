using System.Security.Cryptography;
using System.Text;

namespace game_x.share.Helper;

public static class HashHelper
{
    public static string Sha256(string input)
    {
        if (string.IsNullOrEmpty(input))
            throw new ArgumentException("Input cannot be null or empty.", nameof(input));
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);

        return Convert.ToBase64String(hash);
    }
}
