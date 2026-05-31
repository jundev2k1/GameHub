namespace game_x.application.Features.Games.Client.Commands.Etl998.SearchRecord;

public record SearchRecordCommand(
    DateTime DateStart,
    DateTime DateEnd,
    int PageIndex,
    int PageSize) : ICommand<IReadOnlyCollection<SearchRecordResult>>;
    
public record SearchRecordResult(
    string UserName,
    Etl998GameType GameType,
    int TableId,
    int ShoeNumber,
    int SystemResult,
    int PlayerResult,
    int Tie,
    int BankerPair,
    int PlayerPair,
    int Amount,
    int Result,
    string Ip,
    DateTime BetTime,
    DateTime SettlementTime,
    int BalanceAfter);