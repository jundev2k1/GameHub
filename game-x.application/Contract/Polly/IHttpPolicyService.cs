using Polly;

namespace game_x.application.Contract.Polly;

public interface IHttpPolicyService
{
    IAsyncPolicy<HttpResponseMessage> GetRetryPolicy();
}
