using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.GetAllUnReads;

public sealed class GetAllUnReadsHandler(
    IUserAccessor userAccessor,
    IConversationMemberRepo convMemberRepo
) : IRequestHandler<GetAllUnReadsQuery, IList<ConvUnreadDto>>
{
    public async Task<IList<ConvUnreadDto>> Handle(GetAllUnReadsQuery request, CancellationToken ct)
    {
        var me = userAccessor.GetUserId();
        return await convMemberRepo.GetAllUnReads(me, ct: ct);
    }
}