using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Commands.AssignTalent;

public sealed class AssignTalentHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    ILiveStreamRepo liveStreamRepo) : ICommandHandler<AssignTalentCommand>
{
    public async Task<Unit> Handle(AssignTalentCommand request, CancellationToken ct = default)
    {
        await liveStreamRepo.UpdateAsync(request.Id, async livestream =>
        {
            if (livestream.Status != LiveStreamStatus.Scheduled)
                throw new BadRequestException("Only scheduled livestream can be assigned talent.");

            var talent = await userRepo.GetUserByIdAsync(request.TalentId, ct);
            livestream.AssignStream(talent.Id);

            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        return Unit.Value;
    }
}
