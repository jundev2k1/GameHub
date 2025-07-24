namespace game_x.application.Features.BankAccountManagement.Client.Commands.UpdateDefaultAccount;
public record UpdateDefaultAccountCommand : ICommand
{
    public Guid BankAccountCode { get; init; }
}