using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using Microsoft.AspNetCore.Authorization.Policy;

namespace game_x.api.Middleware;

public sealed class AuthGateMiddleware : IAuthorizationMiddlewareResultHandler
{
    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Succeeded)
        {
            var isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;

            // If the user is authenticated, check if they are blocked or inactive
            Exception? ex = isAuthenticated
                ? await CheckUserStatusAsync(context)
                : null;

            // Next, if the user is blocked or inactive
            if (ex is null)
            {
                await next(context);
                return;
            }

            // If the user is blocked or inactive, handle the exception with a specific message code
            await HandleAuthExceptionAsync(context, ex);
            return;
        }

        if (authorizeResult.Forbidden)
        {
            // If the user is forbidden, handle the exception
            await HandleAuthExceptionAsync(context, new ForbiddenException());
            return;
        }

        if (authorizeResult.Challenged)
        {
            // If the user is not authenticated, handle the exception
            await HandleAuthExceptionAsync(context, new UnauthorizedException());
        }
    }

    private static async Task<Exception?> CheckUserStatusAsync(HttpContext context)
    {
        // Inject the required services
        var userCache = context.RequestServices.GetRequiredService<IUserCacheService>();
        var userAccessor = context.RequestServices.GetRequiredService<IUserAccessor>();

        // Check if the user is blocked or inactive
        var userId = userAccessor.GetUserId();
        var isBlocked = await userCache.IsInactiveUser(userId);
        if (isBlocked) return new ForbiddenException(MessageCode.User.UserDisabled);

        var token = context.Request.Headers.Authorization.ToStringOrEmpty();
        if (token.IsNullOrWhiteSpace())
            return new UnauthorizedException(MessageCode.System.InvalidOrMissingToken);
        if (!IsValidToken(context, userId, token))
            return new UnauthorizedException(MessageCode.System.InvalidOrMissingToken);

        return null;
    }

    private static bool IsValidToken(HttpContext context, string userId, string token)
    {
        // Get the JWT token generator and decode the token
        var tokenGenerator = context.RequestServices.GetRequiredService<IJwtTokenGenerator>();
        var rawToken = token.Split(" ")[1];
        var tokenPayload = tokenGenerator.DecodeToken(rawToken);
        var jwtId = tokenPayload.JwtId;
        if (jwtId.IsNullOrWhiteSpace())
            return false;

        // Check if the token is expired
        if (tokenPayload.IsExpired)
            return false;

        // Check if the token is linked to a valid refresh token
        var refreshTokenManager = context.RequestServices.GetRequiredService<IRefreshTokenManagerCacheService>();
        var tokenLinked = refreshTokenManager.GetTokenByJwtId(userId, jwtId!);
        return tokenLinked != null
            && !tokenLinked.IsExpired
            && !tokenLinked.IsRevoked
            && tokenLinked.ReplacedByToken.IsNullOrEmpty();
    }

    private static async Task HandleAuthExceptionAsync(HttpContext context, Exception ex)
    {
        // Handle specific exceptions and set appropriate status codes
        var (statusCode, messageCode) = ex switch
        {
            UnauthorizedException ue => (StatusCodes.Status401Unauthorized, ue.ErrorCode),
            ForbiddenException => (StatusCodes.Status403Forbidden, MessageCode.System.Forbidden),
            _ => (StatusCodes.Status403Forbidden, MessageCode.System.Forbidden)
        };

        // return a JSON response with the error message
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        var error = ApiResponseFactory.Error(messageCode, statusCode: statusCode);
        await context.Response.WriteAsJsonAsync(error);
    }
}
