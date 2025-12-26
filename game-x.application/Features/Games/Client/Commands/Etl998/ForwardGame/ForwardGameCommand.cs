using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;

namespace game_x.application.Features.Games.Client.Commands.Etl998.ForwardGame;

public record ForwardGameCommand : ICommand<IReadOnlyCollection<ForwardGameResponse>>;