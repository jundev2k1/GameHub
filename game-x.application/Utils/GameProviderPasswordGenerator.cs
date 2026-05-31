using System.Security.Cryptography;

namespace game_x.application.Utils;

internal sealed class GameProviderPasswordGenerator
{
    public static string Generate(int minLength, int maxLength)
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lettersAndDigits = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        // Password length must be between 5 and 12 characters
        int length = RandomNumber(minLength, maxLength); // 13 because Random.Next is exclusive at the upper bound
        char[] password = new char[length];

        using (var rng = RandomNumberGenerator.Create())
        {
            // First character must be an uppercase English letter (A-Z)
            password[0] = upper[RandomNumber(rng, upper.Length)];

            // Remaining characters: letters (upper/lowercase) or digits (0-9)
            // Rule: No consecutive identical characters allowed
            for (int i = 1; i < length; i++)
            {
                char nextChar;
                do
                {
                    nextChar = lettersAndDigits[RandomNumber(rng, lettersAndDigits.Length)];
                }
                while (nextChar == password[i - 1]); // Prevent consecutive duplicate characters

                password[i] = nextChar;
            }
        }

        return new string(password);
    }

    /// <summary>
    /// Generate a random number between min (inclusive) and max (exclusive)
    /// </summary>
    private static int RandomNumber(int min, int max) =>
        RandomNumber(RandomNumberGenerator.Create(), max - min) + min;

    /// <summary>
    /// Generate a random number between 0 (inclusive) and maxExclusive (exclusive) using a secure RNG
    /// </summary>
    private static int RandomNumber(RandomNumberGenerator rng, int maxExclusive)
    {
        byte[] data = new byte[4];
        rng.GetBytes(data);
        return (int)(BitConverter.ToUInt32(data, 0) % (uint)maxExclusive);
    }
}
