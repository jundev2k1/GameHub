using game_x.application.Features.Interactions.Characters.Dtos;

namespace game_x.application.Features.Interactions.Characters.Queries.GetCharacterDetail;

public record GetCharacterDetailQuery(Guid Id) : IQuery<InteractionCharacterDetailDto>;
