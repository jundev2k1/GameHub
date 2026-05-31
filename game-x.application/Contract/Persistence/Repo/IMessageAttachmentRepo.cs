namespace game_x.application.Contract.Persistence.Repo;

public interface IMessageAttachmentRepo
{
    Task AddAsync(MessageAttachment msgAttachment, CancellationToken ct = default);
}