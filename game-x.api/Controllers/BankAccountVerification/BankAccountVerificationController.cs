using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Features.BankAccountVerifications.Commands._1_SubmitBankAccount;

namespace game_x.api.Controllers.BankAccountVerification;

[Route("api/bank-account-verifications")]
public sealed class BankAccountVerificationController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> SubmitAsync([FromForm] SubmitBankAccountRequest request)
    {
        var command = request.Adapt<SubmitBankAccountCommand>() with
        {
            Image = FileUpload.FromFormFile(request.Image)
        };
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result);
    }
}
