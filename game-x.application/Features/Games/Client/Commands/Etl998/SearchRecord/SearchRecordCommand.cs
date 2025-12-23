using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;

namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;

public record SearchRecordCommand(
    string AccountName, 
    string Password,
    string DateStart,
    string DateEnd,
    int PageIndex,
    int PageSize) : ICommand<IReadOnlyCollection<SearchRecordResponse>>;