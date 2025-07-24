using game_x.application.Features.AccountManagement.Staff.Commands.ResetUserPasswordByStaff;
using game_x.application.Features.AccountManagement.Staff.Commands.SendResetUserPasswordCodeByStaff;
using game_x.application.Features.AccountManagement.Staff.Commands.CheckExistedUserInfoByStaff;
using game_x.application.Features.AccountManagement.Staff.Queries.GetUserDetailByStaff;
using game_x.application.Features.AccountManagement.Staff.Commands.CreateUserByStaff;
using game_x.application.Features.AccountManagement.User.Commands.UploadPassportImageByUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using game_x.application.Features.AccountManagement.Dtos;

namespace game_x.api.Controllers.Staff.Client;

[Authorize(Roles = AppRoles.Staff)]
[Route("api/staff/users")]
public class UserController : BaseApiController
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserDetailAsync(string userId)
    {
        var result = await Mediator.Send(new GetUserDetailByStaffQuery(userId));
        return ApiResponseFactory.Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserByStaffCommand byStaffCommand)
    {
        await Mediator.Send(byStaffCommand);
        return ApiResponseFactory.Created();
    }

    [HttpPost("check-availability")]
    public async Task<IActionResult> CheckExistedInfo(CheckExistedUserInfoByStaffCommand byStaffCommand)
    {
        var result = await Mediator.Send(byStaffCommand);
        return ApiResponseFactory.Ok(result);
    }

    /// <summary>
    ///     Sends the reset password verification code Email.
    /// </summary>
    [HttpPost("reset-password/code")]
    public async Task<IActionResult> SendResetPasswordCode(SendResetUserPasswordCodeByStaffCommand byStaffCommand)
    {
        await Mediator.Send(byStaffCommand);
        return ApiResponseFactory.NoContent(MessageCode.System.EmailSendSuccess);
    }

    /// <summary>
    ///     Verifies the verification code and resets the password.
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetUserPasswordByStaffCommand byStaffCommand)
    {
        await Mediator.Send(byStaffCommand);
        return ApiResponseFactory.NoContent(MessageCode.User.UserChangePasswordSuccess);
    }

    [HttpPut("{passportNumber}/update-passport-image")]
    public async Task<IActionResult> UploadPassportImage(string passportNumber, [FromForm] UploadPassportImageRequest request)
    {
        var command = new UploadPassportImageByUserCommand(
            passportNumber,
            File: FileUpload.FromFormFile(request.File));
        var result = await Mediator.Send(command);
        return ApiResponseFactory.Ok(result, MessageCode.System.ImageUploadSuccess);
    }
}
