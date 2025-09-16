using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Identity;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.infrastructure.Identity;

// public sealed class ConversationService(IConversationRepo conversationRepo): IConversationService, IServices
// {
//     public async Task<CursorResult<SupportConversationDto>> GetByCursorAsync(Guid convId, int limit, string? cursor, CancellationToken ct)
//     {
//         var query = await conversationRepo.GetByCursorAsync(convId, limit, cursor, ct);
//         var fp = CursorHelper.ComputeFp($"conv:{convId}");
//
//         var entityResult = await SeekCursorBuilder<MessageDto>
//             .For(query)
//             .Keys(m => m.SentAt, m => m.Id)
//             .Sort(desc1: true, desc2: false)
//             .FromCursor(cursor, fp)
//             .Limit(limit)
//             .ExecuteAsync(m => m, ct);
//
//         var dtoItems = await Task.WhenAll(
//             entityResult.Items.Select(m => GetMessageDtoAsync(m, ct))
//         );
//
//         return entityResult.Transform(dtoItems.Reverse());
//     }
// }