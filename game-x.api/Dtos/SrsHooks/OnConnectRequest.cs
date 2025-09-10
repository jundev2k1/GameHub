using game_x.api.Common;

namespace game_x.api.Dtos.SrsHooks;

public class OnConnectRequest : SrsEventHookRequest
{
    public OnConnectRequest()
    {
        Action = "on_connect";
    }
}
