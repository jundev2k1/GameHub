using game_x.api.Common;

namespace game_x.api.Dtos.SrsHooks;

public class OnUnpublishRequest : SrsEventHookRequest
{
    public OnUnpublishRequest()
    {
        Action = "on_unpublish";
    }
}
