namespace game_x.api.Controllers;

[Route("api/kyc")]
public sealed class KycVerificationController : BaseApiController
{
    [Authorize(Roles = AppRoles.User)]
    [HttpGet("status")]
    public async Task<IActionResult> GetKycStatusAsync()
    {
        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("submit")]
    public async Task<IActionResult> SubmitKycAsync()
    {
        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost("decision")]
    public async Task<IActionResult> DecisionAsync()
    {
        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }

    [Authorize(Roles = AppRoles.User)]
    [HttpPost("resubmit")]
    public async Task<IActionResult> ResubmitAsync()
    {
        await Task.CompletedTask;
        return ApiResponseFactory.NoContent();
    }
}
