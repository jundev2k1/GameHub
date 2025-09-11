using game_x.api.Common;

namespace game_x.api.Dtos.SrsHooks;

public class OnPublishRequest : SrsEventHookRequest
{
    public OnPublishRequest()
    {
        Action = "on_publish";
    }
}
