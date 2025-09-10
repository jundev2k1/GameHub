using game_x.api.Common;

namespace game_x.api.Dtos.SrsHooks;

public class OnPlayRequest : SrsEventHookRequest
{
    public OnPlayRequest()
    {
        Action = "on_play";
    }

    public string PageUrl { get; set; } = string.Empty;
}
