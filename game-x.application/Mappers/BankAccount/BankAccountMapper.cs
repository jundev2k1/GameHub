using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Features.BankAccountManagement.Dtos;
using MapsterMapper;
using BankAccountEntity = game_x.domain.Entities.BankAccount;

namespace game_x.application.Mappers.BankAccount;

public class BankAccountMapper(IMapper mapper)
{
    public BankAccountDetailDto ToDto(BankAccountDetailDto bankAccount)
    {
        return mapper.Map<BankAccountDetailDto>(bankAccount);
    }
public PaginationResult<BankAccountDto> ToBankAccountDtos(PaginationResult<BankAccountEntity> data)
{
    var result = new PaginationResult<BankAccountDto>(
        items: data.Items.Select(item =>
        {
            var dto = mapper.Map<BankAccountDto>(item);
            dto.BankAccountCode = item.PublicId.ToString(); 
            return dto;
        }).ToList(),
        totalItems: data.TotalItems,
        totalPages: data.TotalPages,
        pageIndex: data.PageNumber,
        pageSize: data.PageSize
    );
    return result;
}

}
