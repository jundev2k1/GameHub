using System.Security.Cryptography;
using System.Text;
using game_x.api.Controllers;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.api.Hooks;

[Route("/hooks/test")]
public sealed class TestHookController(
    IOptions<HookTestSettings> settings,
    ILogger<TestHookController> logger) : BaseApiController
{
    [HttpPost("dummy")]
    public async Task<IActionResult> DummyAsync()
    {
        var (valid, payload) = await ValidateSignatureAsync(settings.Value.DummySecretKey);
        if (!valid) return Unauthorized();

        logger.LogInformation("Dummy event processed: {Payload}", payload);

        return Ok();
    }

    private async Task<(bool IsValid, string Payload)> ValidateSignatureAsync(string secretKey)
    {
        Request.EnableBuffering();

        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var payload = await reader.ReadToEndAsync();
        Request.Body.Position = 0;

        var timestamp = Request.Headers["X-Webhook-Timestamp"].FirstOrDefault();
        var signature = Request.Headers["X-Webhook-Signature"].FirstOrDefault();
        var eventType = Request.Headers["X-Webhook-Event"].FirstOrDefault();

        logger.LogInformation("===== WEBHOOK RECEIVED =====");
        logger.LogInformation("Event: {Event}", eventType);
        logger.LogInformation("Timestamp: {Timestamp}", timestamp);
        logger.LogInformation("Payload: {Payload}", payload);
        logger.LogInformation("Signature: {Signature}", signature);

        if (string.IsNullOrEmpty(timestamp) || string.IsNullOrEmpty(signature))
        {
            logger.LogWarning("Missing webhook headers");
            return (false, payload);
        }

        // Anti-replay attack (5 minutes)
        if (!long.TryParse(timestamp, out var ts))
        {
            logger.LogWarning("Invalid timestamp");
            return (false, payload);
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (Math.Abs(now - ts) > 300)
        {
            logger.LogWarning("Webhook timestamp expired");
            return (false, payload);
        }

        var computed = ComputeSignature(timestamp, payload, secretKey);

        logger.LogInformation("Computed Signature: {Signature}", computed);

        var valid = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computed),
            Encoding.UTF8.GetBytes(signature));

        logger.LogInformation("Signature Valid: {Valid}", valid);

        return (valid, payload);
    }

    private static string ComputeSignature(string timestamp, string payload, string secret)
    {
        var signingData = $"{timestamp}.{payload}";

        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(signingData);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}