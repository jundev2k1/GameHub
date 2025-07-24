namespace game_x.application.Features.AccountManagement.Admin.Commands.UpdateStaffStatusByAdmin;

public sealed class UpdateStaffStatusByAdminValidator : AbstractValidator<UpdateStaffStatusByAdminCommand>
{
    public UpdateStaffStatusByAdminValidator()
    {
        RuleFor(x => x.StaffId).NotEmpty().WithMessage("StaffId is required.");
        RuleFor(x => x.Status).IsInEnum().WithMessage("Invalid status.");
    }
}