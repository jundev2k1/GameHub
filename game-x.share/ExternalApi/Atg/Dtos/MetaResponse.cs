namespace game_x.share.ExternalApi.Atg.Dtos;

public sealed record MetaResponse(
    int TotalRecords, 
    int TotalPages, 
    int CurrentPage, 
    bool HasPrevious, 
    bool HasNext);