using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.BankAccountManagement.Dtos;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetBankAccountDetailByUser;

public sealed class GetBankAccountDetailByUserHandler(IBankAccountRepo bankAccountRepo, IUserAccessor userAccessor)
    : IQueryHandler<GetBankAccountDetailByUserQuery, BankAccountDetailDto>
{
    public async Task<BankAccountDetailDto> Handle(GetBankAccountDetailByUserQuery request, CancellationToken ct = default)
    {
        var account = await bankAccountRepo.GetByIdAsync(request.BankAccountCode, ct);
        if (account is null)
            throw new NotFoundException("Bank account not found or has been deleted.");

        var currentUserId = userAccessor.GetUserId();
        if (account.OwnerId != currentUserId)
            throw new ForbiddenException("You are not allowed to access this Bank account.");

        var dto = account.Adapt<BankAccountDetailDto>();
        dto.BankAccountCode = account.PublicId.ToString(); 

        return dto;
    }
}
