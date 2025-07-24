namespace game_x.application.Features.AccountManagement.Admin.Commands.UpdateUserStatusByAdmin;

public sealed class UpdateUserStatusByAdminValidator : AbstractValidator<UpdateUserStatusByAdminCommand>
{
    public UpdateUserStatusByAdminValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status.");
    }
}