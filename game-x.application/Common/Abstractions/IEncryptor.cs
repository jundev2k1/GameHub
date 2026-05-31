namespace game_x.application.Common.Abstractions;

public interface IEncryptor
{
    string Encrypt(string plainText);
    string Decrypt(string base64CipherText);
}