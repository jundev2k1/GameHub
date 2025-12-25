using game_x.share.ExternalApi.Etl998.Dtos.Register;

namespace game_x.application.Features.Games.Client.Commands.Etl998.CreateAccount;

public record CreateAccountCommand(
    string AccountName,
    string Password,
    string Nickname,
    decimal Ximalv,
    int Ximatype,
    int FatherId,
    string Tables) : ICommand<IReadOnlyCollection<Etl998RegisterResponse>>;