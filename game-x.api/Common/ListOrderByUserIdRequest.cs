using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Common;

public sealed class ListOrderByUserIdRequest
{
    [FromQuery(Name = "page")] public int? PageNumber { get; set; } = 1;

    [FromQuery(Name = "pageSize")] public int? PageSize { get; set; } = 20;
}