using game_x.api.Dtos;
using game_x.application.Common.Files;
using game_x.application.Features.Kyc.Commands._1_SubmitKyc;
using game_x.application.Features.Kyc.Commands._2_DecisionKyc;
using game_x.application.Features.Kyc.Commands._3_ResubmitKyc;
using game_x.application.Features.Kyc.Queries.GetKycStatus;

namespace game_x.api.Controllers.Kyc;

[Route("api/kyc")]
public sealed class KycController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpGet("status")]
    public async Task<IActionResult> GetKycStatusAsync()
    {
        var query = new GetKycStatusQuery();
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitKycAsync([FromForm]SubmitKycRequest formData)
    {
        var command = formData.Adapt<SubmitKycCommand>() with
        {
            FrontImage = FileUpload.FromFormFile(formData.FrontPhoto),
            BackImage = FileUpload.FromFormFile(formData.BackPhoto),
        };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("decision")]
    public async Task<IActionResult> DecisionAsync(DecisionKycCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("resubmit")]
    public async Task<IActionResult> ResubmitAsync(ResubmitKycCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent();
    }
}
