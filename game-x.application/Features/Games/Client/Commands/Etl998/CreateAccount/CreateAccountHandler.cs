using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.share.ExternalApi.Etl998.Dtos.CreateAccount;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Games.Client.Commands.Etl998.CreateAccount;

public sealed class CreateAccountHandler(
    IEtl998Service service,
    ILogger<CreateAccountHandler> logger): ICommandHandler<CreateAccountCommand, IReadOnlyCollection<CreateAccountResponse>>
{
    public async Task<IReadOnlyCollection<CreateAccountResponse>> Handle(CreateAccountCommand cmd, CancellationToken ct = default)
    {
        try
        {
            var request = new CreateAccountRequest
            {
                Account = cmd.AccountName,
                Password = cmd.Password,
                Nickname = cmd.Nickname,
                Ximalv = cmd.Ximalv,
                Ximatype = cmd.Ximatype,
                FatherId = cmd.FatherId,
                Tables = cmd.Tables
            };
            return await service.CreateAccountAsync(request);
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to create new etl988 account: {Ex}", ex);
            throw new BadRequestException(MessageCode.System.SystemError, ex.Message);
        }
    }
}