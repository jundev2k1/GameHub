using game_x.api.Common;
using game_x.application.Common.Filters;
using game_x.application.Features.Accounts.Admin.Commands.CreateCustomerSupport;
using game_x.application.Features.Accounts.Admin.Queries.GetCsCriteriaByAdmin;

namespace game_x.api.Controllers.BackOffice.Cs;

[Route("api/back-office/customer-supports")]
public sealed class CustomerSupportController : BaseApiController
{
    [Authorize(Roles = AppRoles.Admin)]
    [HttpGet]
    public async Task<IActionResult> GetCsByCriteria([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetCsCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<IActionResult> CreateCsAsync(CreateCustomerSupportCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }
}
