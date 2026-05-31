using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.GetAllUnReads;

public sealed record GetAllUnReadsQuery(): IQuery<IList<ConvUnreadDto>>;
