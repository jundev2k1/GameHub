using game_x.application.Common.Abstractions.Pagination;
using game_x.application.Common.Filters;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.BankAccountManagement.Dtos;
using game_x.application.Mappers.BankAccount;

namespace game_x.application.Features.BankAccountManagement.Client.Queries.GetBankAccountCriteriaByUser;

public sealed class GetBankAccountCriteriaByUserHandler : IQueryHandler<GetBankAccountByCriteriaQuery, PaginationResult<BankAccountDto>>
{
    private readonly ICriteriaBuilder<BankAccount> _builder;
    private readonly BankAccountMapper _bankAccountMapper;
    private readonly IBankAccountRepo _bankAccountRepo;
     private readonly IUserAccessor _userAccessor;

    public GetBankAccountCriteriaByUserHandler(
        IBankAccountRepo bankAccountRepo,
        ICriteriaBuilder<BankAccount> builder,
        BankAccountMapper bankAccountMapper,
         IUserAccessor userAccessor)
    {
        _bankAccountRepo = bankAccountRepo;
        _builder = builder;
        _bankAccountMapper = bankAccountMapper;
        _userAccessor = userAccessor;

    }

    public async Task<PaginationResult<BankAccountDto>> Handle(GetBankAccountByCriteriaQuery request, CancellationToken ct = default)
    {
        var userId = _userAccessor.GetUserId(); 
        var items = await _bankAccountRepo.GetBankAccountByCriteriaAsync(userId,
            query => _builder.Apply(
                query,
                request.Filters,
                request.Sorts,
                keyword =>
                    bankAccount => bankAccount.BankAccountName.StartsWith(keyword)),
            request.PageIndex ?? 1,
            request.PageSize ?? 20,
            ct);
        var result = _bankAccountMapper.ToBankAccountDtos(items);
        return result;
    }


}