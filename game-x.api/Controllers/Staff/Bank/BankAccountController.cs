using game_x.application.Features.BankAccountManagement.Staff.Queries.GetBankAccountForUserByStaff;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace game_x.api.Controllers.Staff.Bank;

[Authorize(Roles = $"{AppRoles.Staff}")]
[Route("api/staff/users")]
public class BankAccountController : BaseApiController
{
    [HttpGet("{userId}/bank-accounts")]
    public async Task<IActionResult> GetBankAccountByUserIdForStaff(string userId)
    {
        var result = await Mediator.Send(new GetBankAccountsForUserByStaffQuery(userId));
        return ApiResponseFactory.Ok(result);
    }
}
