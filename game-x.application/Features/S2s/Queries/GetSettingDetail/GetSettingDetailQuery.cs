using game_x.application.Features.S2s.DTOs;

namespace game_x.application.Features.S2s.Queries.GetSettingDetail;

public record GetSettingDetailQuery(string ClientId, string AppCode) : IQuery<S2sClientSettingDetailDto>;
