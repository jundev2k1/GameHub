namespace game_x.api.Common;

public sealed class AnchorWindowRequest
{
    [FromQuery(Name = "before")]
    public int? Before { get; set; }
    [FromQuery(Name = "after")]
    public int? After { get; set; }
    [FromQuery(Name = "anchor")]
    public WindowAnchorType? Anchor { get; set; }
}