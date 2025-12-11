using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.share.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace game_x.infrastructure.Security.HMac;

public sealed class HmacValidator(
    IOptions<HmacSettings> options,
    IOptions<BaccaratHmacSettings> baccaratOptions,
    IHmacNonceStore nonceStore,
    IAppLogger<HmacValidator> logger) : IHmacValidator
{
    public async Task<bool> ValidateAsync(HttpRequest request, CancellationToken ct = default)
    {
        logger.LogInformation("|===========> 1");
        if (!request.Headers.TryGetValue(options.Value.PartnerHeader, out var partnerValues))
            return false;
        if (!request.Headers.TryGetValue(options.Value.SignatureHeader, out var signatureValues))
            return false;
        if (!request.Headers.TryGetValue(options.Value.TimestampHeader, out var tsValues))
            return false;
        if (!request.Headers.TryGetValue(options.Value.NonceHeader, out var nonceValues))
            return false;

        logger.LogInformation("|===========> 2");
        var signature = signatureValues.First();
        var tsRaw = tsValues.First();
        var nonce = nonceValues.First();
        var partnerName = partnerValues.First();

        var secretKey = partnerName switch
        {
            var name when name == PartnerName.Baccarat => baccaratOptions.Value.Secret,
            _ => null
        };
        if (secretKey is null) return false;

        logger.LogInformation("|===========> 3");
        if (!long.TryParse(tsRaw, out var tsSeconds))
            return false;

        logger.LogInformation("|===========> 4");
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - tsSeconds) > options.Value.AllowedTimestampSkewSeconds)
            return false; // timestamp out of allowed window

        logger.LogInformation("|===========> 5");
        // Nonce check (replay protection)
        var ttl = TimeSpan.FromSeconds(options.Value.NonceTtlSeconds);
        var isNew = await nonceStore.TryUseNonceAsync(nonce!, ttl);
        if (!isNew) return false;

        logger.LogInformation("|===========> 6");
        // Read body (we need to allow stream to be read by proxy after validation)
        request.EnableBuffering();
        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        request.Body.Position = 0;

        // Compute body SHA256 hash → base64 (same as client)
        var bodyBytes = Encoding.UTF8.GetBytes(body);
        var bodyHash = Convert.ToBase64String(SHA256.HashData(bodyBytes));
        var method = request.Method;
        var pathAndQuery = request.Path + request.QueryString;

        // Build canonical string to sign. Adjust order and canonicalization as needed.
        // Here: method + path + query + timestamp + nonce + body
        var canonical = $"{tsRaw}\n{nonce}\n{method}\n{pathAndQuery}\n{bodyHash}";

        // Compute HMAC
        var computed = ComputeHmac(secretKey, canonical);

        // Compare in constant time
        return SecureEquals(computed, signature!);
    }

    private static string ComputeHmac(string secret, string message)
    {
        byte[] key;
        try
        {
            key = Convert.FromBase64String(secret);
        }
        catch
        {
            key = Encoding.UTF8.GetBytes(secret);
        }
        var msg = Encoding.UTF8.GetBytes(message);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(msg);
        return Convert.ToBase64String(hash);
    }

    private static bool SecureEquals(string a, string b)
    {
        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);
        if (aBytes.Length != bBytes.Length) return false;

        var diff = 0;
        for (int i = 0; i < aBytes.Length; i++) diff |= aBytes[i] ^ bBytes[i];
        return diff == 0;
    }
}
