using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Persistence.Repo;

public interface IMessageRepo
{
    Task AddAsync(Message msg, CancellationToken ct = default);

    /// <summary> List/Seek (bidirectional) </summary>
    Task<IQueryable<MessageDto>> GetByCursorAsync(
        Guid convId, 
        int limit, 
        string? cursor, 
        CancellationToken ct = default);

    /// <summary> Window/Anchor (jump to message) </summary>
    Task<MessageWindowDto> GetWindowAsync(
        Guid convId, Guid anchorId, int before, int after, WindowAnchorType anchor, CancellationToken ct = default);
    
    Task<Message?> CheckExistByConvIdAsync(Guid convId, Guid msgId, CancellationToken ct = default);
    Task<Message> CheckExistAsync(Guid id, CancellationToken ct = default);
    Task<Message> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Message?> GetLastedMessageAsync(int convId, CancellationToken ct = default);
}