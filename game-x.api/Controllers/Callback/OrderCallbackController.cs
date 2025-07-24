using game_x.application.Features.OrderManagement.Shared.Commands.Callback.Fiat;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Callback;

[Route("api/callback/order")]
public class OrderCallbackController : BaseApiController
{
    [HttpPost("fiat")]
    public async Task<IActionResult> FiatCallback(SecureRequest<FiatCallbackRequest> request)
    {
        var command = new UpdateOrderCallbackCommand(
            Data: request.Data,
            Signature: request.Signature);
        var result = await Mediator.Send(command);
        return Ok(result);
    }
}
