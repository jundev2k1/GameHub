using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.S2s.Commands.RevokeCredentialSetting;

public sealed class RevokeCredentialSettingHandler(
    IUnitOfWork unitOfWork,
    IS2sCredentialRepo s2SCredentialRepo) : ICommandHandler<RevokeCredentialSettingCommand>
{
    public async Task<Unit> Handle(RevokeCredentialSettingCommand request, CancellationToken ct= default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await s2SCredentialRepo.UpdateAsync(request.KeyId, async credential =>
            {
                if (credential.ClientSetting.AppCode != request.AppCode
                    || credential.ClientSetting.ClientId != request.ClientId)
                    throw new BadRequestException($"AppCode({request.AppCode}) or ClientId({request.ClientId}) are invalid.");

                if (credential.Status != CredentialStatus.Active)
                    throw new BadRequestException(MessageCode.System.InvalidResourceState);

                credential.Revoke();
                await Task.CompletedTask;
            }, ct);
        }, ct);

        return Unit.Value;
    }
}
