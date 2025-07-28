using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Identity;

namespace game_x.application.Features.Auth.Root.Commands.RootLogin;

public sealed class RootLoginHandler(
    IJwtTokenGenerator jwtTokenGenerator,
    IAuthService authService) : ICommandHandler<RootLoginCommand, RootLoginResult>
{
    public async Task<RootLoginResult> Handle(RootLoginCommand request, CancellationToken ct)
    {
        var user = await authService.TryLoginAsync(request.UserName, request.Password);
        var role = await authService.GetRolesAsync(user);
        if (!role.IsRoot)
            throw new ForbiddenException();

        var result = await jwtTokenGenerator.GenerateToken(user);
        return new RootLoginResult(Token: result.Token);
    }
}
