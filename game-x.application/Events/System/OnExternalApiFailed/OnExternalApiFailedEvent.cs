namespace game_x.application.Events.System.OnExternalApiFailed;

public enum ExternalApiAction
{
    SyncWallet = 0,
}

public record OnExternalApiFailedEvent(string UserId, Guid PlatformId, ExternalApiAction Action, string Message) : IApplicationEvent;
