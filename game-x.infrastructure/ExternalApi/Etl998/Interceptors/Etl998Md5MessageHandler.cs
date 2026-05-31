using game_x.share.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using game_x.infrastructure.ExternalApi.Etl998.Enums;

namespace game_x.infrastructure.ExternalApi.Etl998.Interceptors;

public sealed class Etl998Md5MessageHandler(IOptions<Etl998Settings> settings): DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.Method != HttpMethod.Post)
            throw new InvalidOperationException("ETL998 API only supports HTTP POST.");

        var json = request.Content == null
            ? "{}"
            : await request.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
        
        json = InjectMd5Key(json, settings.Value.Md5Key);
        
        var data = Etl998Security.DesEncryptToBase64(json, settings.Value.DesKey);
        var sign = Etl998Security.ComputeSignProvider(data);
        
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["data"] = data,
            ["sign"] = sign,
            ["agentId"] = settings.Value.AgentId
        });

        if(!request.Headers.Accept.Any())
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return await base.SendAsync(request, ct).ConfigureAwait(false);
    }
    
    // Keep whatever property names are already in the JSON (do not camelCase / rename).
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = null };
    
    private static string InjectMd5Key(string json, string md5Key)
    {
        using var doc = JsonDocument.Parse(json);
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
            throw new InvalidOperationException("ETL998 request body must be a JSON object.");

        var dict = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var p in doc.RootElement.EnumerateObject()) { dict[p.Name] = p.Value.Clone(); }
        dict["md5key"] = md5Key;
        dict["actype"] = Etl998AccountType.Real;
        return JsonSerializer.Serialize(dict, JsonOptions);
    }
}