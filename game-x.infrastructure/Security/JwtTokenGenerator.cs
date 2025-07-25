using game_x.application.Contract.Infrastructure.Security;
using game_x.domain.Entities;
using game_x.share.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace game_x.infrastructure.Security;
public class JwtTokenGenerator(
    UserManager<User> userManager,
    IOptions<JwtSettings> jwtOptions) : IJwtTokenGenerator
{
    public async Task<JwtTokenDto> GenerateToken(User user)
    {
        var jwtSettingsValue = jwtOptions.Value;

        var userClaims = await userManager.GetClaimsAsync(user);
        var roles = await userManager.GetRolesAsync(user);
        var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

        var jti = Guid.NewGuid().ToString();

        await userManager.RemoveAuthenticationTokenAsync(user, jwtSettingsValue.Provider, jwtSettingsValue.Version);
        await userManager.SetAuthenticationTokenAsync(user, jwtSettingsValue.Provider, jwtSettingsValue.Version, jti);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, jti),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        }.Union(userClaims).Union(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettingsValue.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddMinutes(jwtSettingsValue.DurationInMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtSettingsValue.Issuer,
            audience: jwtSettingsValue.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtTokenDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expires
        };
    }
}
