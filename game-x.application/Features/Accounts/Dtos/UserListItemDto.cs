
namespace game_x.application.Features.Accounts.Dtos;

public class UserListItemDto : UserDto
{
    public decimal Balance { get; set; }
    public decimal FrozenBalance { get; set; }
    public decimal TotalBalance => Balance + FrozenBalance;
    public decimal GamePoint { get; set; }
    public decimal LockedGamePoint { get; set; }
    public decimal TotalGamePoint => GamePoint + LockedGamePoint;
}