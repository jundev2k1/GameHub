namespace game_x.domain.Enum;

public enum AuthMethod : short
{
    HmacSha256 = 1,
    RsaSha256 = 2,
    EcdsaP256 = 3,
    EcdsaP384 = 4
}
