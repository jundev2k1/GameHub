using Microsoft.AspNetCore.Http;

namespace game_x.application.Contract.Infrastructure.Security;

public interface IHmacValidator
{
    Task<bool> ValidateAsync(HttpRequest request, CancellationToken cancellationToken = default);
}
