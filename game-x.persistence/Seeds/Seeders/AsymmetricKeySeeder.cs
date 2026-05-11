using game_x.application.Contract.Infrastructure.Security;
using game_x.domain.Constants;

namespace game_x.persistence.Seeds.Seeders;

public sealed class AsymmetricKeySeeder(IAsymmetricCryptoService cryptoService) : ISeeder
{
    public async Task SeedAsync(GameXContext context, CancellationToken ct = default)
    {
        if (await context.AsymmetricKeys.AnyAsync(ct)) return;

        var (publicKeyPem, privateKeyPem) = cryptoService.GenerateKeyPair();

        var keys = new List<AsymmetricKey>
        {
            AsymmetricKey.Create(
                AsymmetricKeyNames.GameX,
                AsymmetricKeyType.Private,
                AsymmetricType.ECDSA,
                privateKeyPem,
                "GameX 系統簽名用私鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.GameX,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                publicKeyPem,
                "GameX 公鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.GalaxyPay,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAE3wb6KaEwU9zFGqn6/BmH7T+Wekcs\r\nEMi2RnMWLztrnfF3Ck0O8G5s88jm0Zhq15t9W7VA0Qu3sr/6WFoZjS2c7g==\r\n-----END PUBLIC KEY-----",
                "Galaxy Pay 公鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.Uxm,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEm1PhAmoUuAmQANNJFJov1Dra6kXt\r\nMM7OcxKGd0qtCZgNT375AasOYAKqxlhGZHX8ohfIF+Pa1bfbysSujYKGRw==\r\n-----END PUBLIC KEY-----",
                "Uxm 公鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.FastPay,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                "-----BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEdWB7ib7rd4ofhNF14mzPuONY/RGX\r\nnYyH0X+tvA3CxLosHg7RXf3dA++DImP7vJMA4mGXFtuK5G4TRgCP4/wE6Q==\r\n-----END PUBLIC KEY---—",
                "Fastpay 公鑰"),
            AsymmetricKey.Create(
                AsymmetricKeyNames.Slot,
                AsymmetricKeyType.Public,
                AsymmetricType.ECDSA,
                "----- BEGIN PUBLIC KEY-----\r\nMFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEm1PhAmoUuAmQANNJFJov1Dra6kXt\r\nMM7OcxKGd0qtCZgNT375AasOYAKqxlhGZHX8ohfIF+Pa1bfbysSujYKGRw==\r\n-----END PUBLIC KEY-----",
                "Uxm 公鑰"),
        };

        await context.AsymmetricKeys.AddRangeAsync(keys, ct);
    }
}