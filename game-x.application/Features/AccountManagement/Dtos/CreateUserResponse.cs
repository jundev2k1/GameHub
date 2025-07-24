namespace game_x.application.Features.AccountManagement.Dtos;

public class CreateUserResponse
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }

        
    internal CreateUserResponse(string userId, string username, string password)
    {
        UserId = userId;
        UserName = username;
        Password = password;
    }
}