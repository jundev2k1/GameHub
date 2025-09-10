using game_x.api.Common;

namespace game_x.api.Dtos.SrsHooks;

public class OnDisconnectRequest : SrsEventHookRequest
{
    public OnDisconnectRequest()
    {
        Action = "on_disconnect";
    }
}
