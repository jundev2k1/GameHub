using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.AuditLogs.Dtos;

namespace game_x.application.Features.AuditLogs.Mapping;

public static class AuditLogMapping
{
    public static PaginationResult<AuditLogDto> ToSearchResult(this PaginationResult<AuditLog> data)
    {
        var result = new PaginationResult<AuditLogDto>(
            items: [.. data.Items.Select(item => item.Adapt<AuditLogDto>())],
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
