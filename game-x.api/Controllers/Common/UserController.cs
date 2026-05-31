using game_x.application.Features.Accounts.Common.Queries.GetUserWithSuggestions;

namespace game_x.api.Controllers.Common;

[Route("api/users")]
public sealed class UserController : BaseApiController
{
    [HttpGet("suggestions")]
    public async Task<IActionResult> GetUserWithSuggestionsAsync([FromQuery] GetUserWithSuggestionsQuery query)
    {
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
