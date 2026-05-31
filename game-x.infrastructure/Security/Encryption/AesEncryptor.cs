using System.Security.Cryptography;
using System.Text;

namespace game_x.infrastructure.Security.Encryption;

public abstract class AesEncryptor(string aesKey, string iv)
{
    private readonly byte[] _key = Encoding.UTF8.GetBytes(aesKey);
    private readonly byte[] _iv = Encoding.UTF8.GetBytes(iv);

    public virtual string Encrypt(string plainText)
    {
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _key;
        aes.IV = _iv;

        var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        return Convert.ToBase64String(cipherBytes);
    }

    public virtual string Decrypt(string base64CipherText)
    {
        var cipherBytes = Convert.FromBase64String(base64CipherText);
        using var aes = Aes.Create();
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = _key;
        aes.IV = _iv;

        var decryptor = aes.CreateDecryptor();
        var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
