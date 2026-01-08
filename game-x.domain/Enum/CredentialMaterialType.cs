namespace game_x.domain.Enum;

public enum CredentialMaterialType : short
{
    EcdsaPublicKey = 0,
    EcdsaPrivateKey = 1,
    RsaPublicKey = 2,
    RsaPrivateKey = 3,
    ApiKey = 4,
    HmacSecret = 5,
}
