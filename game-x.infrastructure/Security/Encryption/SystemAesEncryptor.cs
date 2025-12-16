using game_x.application.Contract.Infrastructure.Security;
using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace game_x.infrastructure.Security.Encryption;

public sealed class SystemAesEncryptor(IOptions<GameXSettings> options)
    : AesEncryptor(options.Value.AesKey, string.Empty), IAesEncryptor
{
    private readonly byte[] _key = Convert.FromBase64String(options.Value.AesKey);

    public override string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _key;
        aes.GenerateIV();

        var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // prepend IV to ciphertext
        var result = new byte[aes.IV.Length + cipherBytes.Length];
        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
        Buffer.BlockCopy(cipherBytes, 0, result, aes.IV.Length, cipherBytes.Length);

        return Convert.ToBase64String(result);
    }

    public override string Decrypt(string base64CipherText)
    {
        var full = Convert.FromBase64String(base64CipherText);

        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _key;

        var iv = new byte[16];
        var cipherBytes = new byte[full.Length - iv.Length];

        Buffer.BlockCopy(full, 0, iv, 0, iv.Length);
        Buffer.BlockCopy(full, iv.Length, cipherBytes, 0, cipherBytes.Length);

        aes.IV = iv;

        var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
