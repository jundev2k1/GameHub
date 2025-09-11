using game_x.api.Common;

namespace game_x.api.Dtos.SrsHooks;

public class OnStopRequest : SrsEventHookRequest
{
    public OnStopRequest()
    {
        Action = "on_stop";
    }
}
