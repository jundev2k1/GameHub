using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using System.Text;

namespace game_x.infrastructure.ExternalApi.SasSlot.Intercepters;

public sealed class SlotMessageHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            var rawBody = await request.Content.ReadAsStringAsync(cancellationToken);

            var uxmPublicKey = asymmetricKeyCacheService.SlotPublicKey;
            var signature = asymmetricCryptoService.Sign(uxmPublicKey, rawBody);

            // Replace body with canonical JSON
            request.Content = new StringContent(
                rawBody,
                Encoding.UTF8,
                "application/json"
            );

            // Headers
            request.Headers.Add("X-Signature-Alg", "ES256");
            request.Headers.Add("X-Signature", signature);
            request.Headers.Add("X-Key-Id", "nmy-2025-12");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
