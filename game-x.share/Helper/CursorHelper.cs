using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Security.Cryptography;

namespace game_x.share.Helper;

/// <summary>Cursor helpers for opaque seek pagination (2-key: UTC DateTime + int).</summary>
public static class CursorHelper
{
    public enum Dir { Older, Newer }

    // Opaque payload inside the cursor token (short keys for compact tokens).
    public sealed record Payload(
        [property: JsonPropertyName("v")] string V, // version, e.g. "k2-1"
        [property: JsonPropertyName("d")] Dir D, // direction
        [property: JsonPropertyName("k1")] long K1, // primary UTC ticks
        [property: JsonPropertyName("k2")] int K2, // secondary id
        [property: JsonPropertyName("f")] string? F = null // filter fingerprint
    );

    public static class Token
    {
        private static readonly JsonSerializerOptions Opt = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static string Encode(Payload p)
        {
            var json = JsonSerializer.Serialize(p, Opt);
            return Base64Url.Encode(Encoding.UTF8.GetBytes(json));
        }

        public static bool TryDecode(string? token, out Payload? p)
        {
            p = null;
            if (string.IsNullOrWhiteSpace(token)) return false;
            try
            {
                var json = Encoding.UTF8.GetString(Base64Url.Decode(token));
                p = JsonSerializer.Deserialize<Payload>(json, Opt);
                return p is not null;
            }
            catch { return false; }
        }

        // Build token directly from keys.
        public static string EncodeKeys(
            DateTime k1Utc,
            int k2,
            Dir d = Dir.Older,
            string v = "k2-1",
            string? f = null)
        {
            if (k1Utc.Kind != DateTimeKind.Utc)
                throw new ArgumentException("k1Utc must be UTC.", nameof(k1Utc));
            return Encode(new Payload(v, d, k1Utc.Ticks, k2, f));
        }
    }

    // Web-safe Base64 without padding.
    private static class Base64Url
    {
        public static string Encode(byte[] input)
        {
            var s = Convert.ToBase64String(input);
            return s.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static byte[] Decode(string input)
        {
            var s = input.Replace('-', '+').Replace('_', '/');

            // Fix: compute mod first, then switch to add padding
            var mod = s.Length % 4;
            switch (mod)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
                case 0: break;
                default: throw new FormatException("Invalid base64url length.");
            }

            return Convert.FromBase64String(s);
        }
    }

    /// <summary>Short fingerprint for filter params (detects mismatched cursor reuse).</summary>
    public static string ComputeFp(string raw)
    {
        using var sha = SHA256.Create();
        var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
        Span<byte> short8 = stackalloc byte[8];
        hash.AsSpan(0, 8).CopyTo(short8);
        return Base64Url.Encode(short8.ToArray());
    }
}
