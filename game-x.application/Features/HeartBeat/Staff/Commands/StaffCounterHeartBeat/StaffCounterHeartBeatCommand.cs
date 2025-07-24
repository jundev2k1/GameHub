namespace game_x.application.Features.HeartBeat.Staff.Commands.StaffCounterHeartBeat;

public record StaffCounterHeartBeatCommand(Guid SessionKey) : ICommand;
