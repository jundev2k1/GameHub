using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace game_x.infrastructure.ExternalApi.GameBaccarat.Intercepters;

public sealed class HmacMessageHandler : DelegatingHandler
{
    private readonly BaccaratHmacSettings _settings;
    private readonly byte[] _secretKey;

    public HmacMessageHandler(IOptions<BaccaratHmacSettings> settings)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        if (string.IsNullOrEmpty(_settings.Secret))
            throw new ArgumentException("HMAC secret is required in HmacSettings.");

        // Accept either base64-encoded secret or raw UTF-8 secret
        try
        {
            _secretKey = Convert.FromBase64String(_settings.Secret);
        }
        catch
        {
            _secretKey = Encoding.UTF8.GetBytes(_settings.Secret);
        }
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct = default)
    {
        // Timestamp (unix seconds)
        var ts = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        // Nonce: 16 bytes (base64)
        var nonce = GenerateNonceBase64(16);
        // Build path + query (relative path)

        // Path + query
        var pathAndQuery = request.RequestUri?.PathAndQuery ?? "/";

        // Method
        var method = request.Method.Method.ToUpperInvariant();

        // Read body and compute body hash
        string bodyHash;
        if (request.Content == null)
        {
            bodyHash = ComputeSha256HashBase64(string.Empty);
        }
        else
        {
            // Ensure content can be read multiple times by buffering
            var contentBytes = await request.Content.ReadAsByteArrayAsync(ct).ConfigureAwait(false);

            // Rewind content by creating new ByteArrayContent and copying headers
            var newContent = new ByteArrayContent(contentBytes);
            foreach (var header in request.Content.Headers)
                newContent.Headers.TryAddWithoutValidation(header.Key, header.Value);

            request.Content = newContent;

            // Compute body hash
            bodyHash = ComputeSha256HashBase64(contentBytes);
        }

        // Canonical string: timestamp + newline + method + newline + path + newline + bodyHash
        var canonical = $"{ts}\n{nonce}\n{method}\n{pathAndQuery}\n{bodyHash}";

        // Compute signature: HMACSHA256(secret, canonical) -> base64
        var signature = ComputeHmacSha256Base64(_secretKey, canonical);

        // Add headers
        if (!string.IsNullOrEmpty(_settings.ClientIdHeader))
        {
            request.Headers.Remove(_settings.ClientIdHeader);
            request.Headers.Add(_settings.ClientIdHeader, _settings.ClientId);
        }

        if (!string.IsNullOrEmpty(_settings.AppIdHeader))
        {
            request.Headers.Remove(_settings.AppIdHeader);
            request.Headers.Add(_settings.AppIdHeader, _settings.AppId);
        }

        if (!string.IsNullOrEmpty(_settings.TimestampHeader))
        {
            request.Headers.Remove(_settings.TimestampHeader);
            request.Headers.Add(_settings.TimestampHeader, ts);
        }

        if (!string.IsNullOrEmpty(_settings.NonceHeader))
        {
            request.Headers.Remove(_settings.NonceHeader);
            request.Headers.Add(_settings.NonceHeader, nonce);
        }

        if (!string.IsNullOrEmpty(_settings.SignatureHeader))
        {
            request.Headers.Remove(_settings.SignatureHeader);
            request.Headers.Add(_settings.SignatureHeader, signature);
        }

        return await base.SendAsync(request, ct).ConfigureAwait(false);
    }

    private static string GenerateNonceBase64(int length)
    {
        var bytes = new byte[length];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string ComputeSha256HashBase64(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        return ComputeSha256HashBase64(bytes);
    }

    private static string ComputeSha256HashBase64(byte[] bytes)
    {
        var hash = SHA256.HashData(bytes);
        return Convert.ToBase64String(hash);
    }

    private static string ComputeHmacSha256Base64(byte[] key, string message)
    {
        using var hmac = new HMACSHA256(key);
        var messageBytes = Encoding.UTF8.GetBytes(message);
        var mac = hmac.ComputeHash(messageBytes);
        return Convert.ToBase64String(mac);
    }
}
