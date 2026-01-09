namespace game_x.application.Contract.Infrastructure.Security;

public interface IAuthGenerator
{
    (string PublicKey, string PrivateKey) GenerateEcdsaKeys();

    (string PublicKey, string PrivateKey) GenerateRsaKeys();

    string GenerateHmacKey();

    string GenerateApiKey(int keyLength = 32);
}
