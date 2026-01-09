using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Security;
using System.Security.Cryptography;

namespace game_x.infrastructure.Security;

public sealed class AuthGenerator(IAsymmetricCryptoService asymmetricCrypto) : IAuthGenerator, IServices
{
    public (string PublicKey, string PrivateKey) GenerateEcdsaKeys()
    {
        return asymmetricCrypto.GenerateKeyPair();
    }

    public (string PublicKey, string PrivateKey) GenerateRsaKeys()
    {
        // TODO (RSA Key): Add logic to generate API key
        return GenerateEcdsaKeys();
    }

    public string GenerateHmacKey()
    {
        // 32 bytes = 256-bit
        var bytes = new byte[32];
        RandomNumberGenerator.Fill(bytes);

        return Convert.ToBase64String(bytes);
    }

    public string GenerateApiKey(int keyLength = 32)
    {
        var bytes = new byte[keyLength];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        // Convert to URL-safe Base64
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}
