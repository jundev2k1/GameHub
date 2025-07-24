namespace game_x.application.Features.AccountManagement.Dtos;

public sealed class UserStatisticsDto
{
    public int AllUser { get; set; }
    public int InactiveUser { get; set; }
    public int ActiveUser { get; set; }
    public int NewUser { get; set; }

}
