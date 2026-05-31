using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.User.Commands.UserSelfUpdate;

public sealed class UserSelfUpdateHandler(IUnitOfWork unitOfWork, IUserRepo userRepo, IUserAccessor userAccessor)
    : ICommandHandler<UserSelfUpdateCommand>
{
    public async Task<Unit> Handle(UserSelfUpdateCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();

        await userRepo.UpdateAsync(userId, user =>
        {
            user.PhoneNumber = request.PhoneNumber;
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}
