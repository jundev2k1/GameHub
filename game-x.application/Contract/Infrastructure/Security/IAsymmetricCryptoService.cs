namespace game_x.application.Contract.Infrastructure.Security;

public interface IAsymmetricCryptoService
{
    (string PublicKeyPem, string PrivateKeyPem) GenerateKeyPair();
    string Sign(string privateKeyPem, object data);
    bool VerifySignature(string publicKeyPem, object data, string signatureBase64);
}
