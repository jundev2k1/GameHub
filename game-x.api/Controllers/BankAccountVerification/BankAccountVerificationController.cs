using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Exceptions;
using game_x.application.Features.BankAccountVerifications.Commands._1_SubmitBankAccount;
using game_x.application.Features.BankAccountVerifications.Commands._2_DecisionBankAccount;
using game_x.application.Features.BankAccountVerifications.Commands._3_ResubmitBankAccount;
using game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountProfile;
using game_x.domain.ValueObjects;

namespace game_x.api.Controllers.BankAccountVerification;

[Route("api/bank-account-verifications")]
public sealed class BankAccountVerificationController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpGet("me/{currency}")]
    public async Task<IActionResult> GetBankAccountProfileAsync(string currency)
    {
        var query = new GetBankAccountProfileQuery(currency);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost]
    public async Task<IActionResult> SubmitAsync([FromForm] SubmitBankAccountRequest request)
    {
        var command = request.Adapt<SubmitBankAccountCommand>() with
        {
            Image = FileUpload.FromFormFile(request.Image)
        };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPatch("{code}/resubmit")]
    public async Task<IActionResult> ReSubmitAsync(string code, [FromForm] ReSubmitBankAccountRequest request)
    {
        if (!CurrencyUnit.IsValid(code))
            throw new BadRequestException(MessageCode.System.InvalidParameters);

        var command = request.Adapt<ResubmitBankAccountCommand>() with
        {
            CurrencyCode = code,
            Image = request.Image != null ? FileUpload.FromFormFile(request.Image) : null
        };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Cs}")]
    [HttpPatch("{id:guid}/decision")]
    public async Task<IActionResult> DecideAsync(Guid id, DecisionBankAccountCommand request)
    {
        var command = request.Adapt<DecisionBankAccountCommand>() with { Id = id };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
