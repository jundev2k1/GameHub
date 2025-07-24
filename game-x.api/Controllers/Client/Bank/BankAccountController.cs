using game_x.application.Features.BankAccountManagement.Client.Commands.CreateBankAccount;
using game_x.application.Features.BankAccountManagement.Client.Commands.DeleteBankAccount;
using game_x.application.Features.BankAccountManagement.Client.Commands.UpdateBankAccount;
using game_x.application.Features.BankAccountManagement.Client.Commands.UpdateDefaultAccount;
using game_x.application.Features.BankAccountManagement.Client.Queries.GetSelfUserBankAccount;
using game_x.application.Features.BankAccountManagement.Client.Queries.GetBankAccountDetailByUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Client.Bank;

[Authorize(Roles = $"{AppRoles.User}")]
[Route("api/user/bank-accounts")]
public class BankAccountController : BaseApiController
{
    [HttpGet("{bankAccountCode}")]
    public async Task<IActionResult> GetDetail(Guid bankAccountCode)
    {
        var result = await Mediator.Send(new GetBankAccountDetailByUserQuery(bankAccountCode));
        return ApiResponseFactory.Ok(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMyBankAccounts()
    {
        var result = await Mediator.Send(new GetSelfUserBankAccountsQuery());
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBankAccount(CreateBankAccountCommand command)
    {
        await Mediator.Send(command);
        return ApiResponseFactory.Created();
    }

    [HttpPatch("{bankAccountCode}/default")]
    public async Task<IActionResult> UpdateDefaultAccount(Guid bankAccountCode)
    {
        var command = new UpdateDefaultAccountCommand { BankAccountCode = bankAccountCode };
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }

    [HttpDelete("{bankAccountCode}")]
    public async Task<IActionResult> DeleteBankAccountAsync(Guid bankAccountCode)
    {
        var command = new DeleteBankAccountCommand(bankAccountCode);
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Deleted);
    }

    [HttpPatch("{bankAccountCode}")]
    public async Task<IActionResult> UpdateBankAccount(Guid bankAccountCode, UpdateBankAccountCommand command)
    {
        command.BankAccountCode = bankAccountCode;
        await Mediator.Send(command);
        return ApiResponseFactory.NoContent(code: MessageCode.System.Updated);
    }
}
