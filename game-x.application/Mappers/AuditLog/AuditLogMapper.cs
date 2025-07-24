using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.AuditLogManagement.Dtos;
using AuditLogEntity = game_x.domain.Entities.AuditLog;

namespace game_x.application.Mappers.BankAccount;

public class AuditLogMapper()
{
    public PaginationResult<AuditLogDto> ToSearchResult(PaginationResult<AuditLogEntity> data)
    {
        var result = new PaginationResult<AuditLogDto>(
            items: data.Items.Select(item => item.Adapt<AuditLogDto>()).ToList(),
            totalItems: data.TotalItems,
            totalPages: data.TotalPages,
            pageIndex: data.PageNumber,
            pageSize: data.PageSize
        );
        return result;
    }
}
