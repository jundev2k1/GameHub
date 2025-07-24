using game_x.application.Features.AuditLogManagement.Admin.Queries.GetAuditCriteriaByAdmin;
using game_x.application.Features.AuditLogManagement.Admin.Queries.GetAuditDetailByAdmin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Admin.AuditLog;

[Authorize(Roles = AppRoles.Admin)]
[Route("api/admin/audits")]
public class AuditLogController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetByCriteriaAsync([AsParameters] SearchCriteriaRequest parameters)
    {
        var filters = QueryConverter.ToFilters(parameters.Filters, parameters.Keyword);
        var sorts = QueryConverter.ToSorts(parameters.Sorts);
        var query = new GetAuditCriteriaByAdminQuery(
            filters,
            sorts,
            parameters.PageNumber,
            parameters.PageSize);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByAuditLogId(Guid id)
    {
        var query = new GetAuditDetailByAdminQuery(id);
        var result = await Mediator.Send(query);
        return ApiResponseFactory.Ok(result);
    }
}
