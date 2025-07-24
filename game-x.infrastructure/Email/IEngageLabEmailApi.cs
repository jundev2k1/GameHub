
using game_x.application.Contract.Infrastructure.Email;
using Refit;

namespace game_x.infrastructure.Email;

public interface IEngageLabEmailApi
{
    [Post("/v1/mail/send")]
    Task<ApiResponse<EngageLabEmailResponse>> SendEmailAsync([Body] EngageLabEmailRequest request);
}