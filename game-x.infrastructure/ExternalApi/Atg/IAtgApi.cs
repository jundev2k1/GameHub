using game_x.share.ExternalApi.Atg.Dtos;
using game_x.share.ExternalApi.Atg.Dtos.AccessToken;
using game_x.share.ExternalApi.Atg.Dtos.GetGameBalance;
using game_x.share.ExternalApi.Atg.Dtos.GameProvider;
using game_x.share.ExternalApi.Atg.Dtos.GetGameKey;
using game_x.share.ExternalApi.Atg.Dtos.GetGameLobby;
using game_x.share.ExternalApi.Atg.Dtos.GetGames;
using game_x.share.ExternalApi.Atg.Dtos.PlayGame;
using game_x.share.ExternalApi.Atg.Dtos.Register;
using game_x.share.ExternalApi.Atg.Dtos.UpdateGameBalance;
using Refit;

namespace game_x.infrastructure.ExternalApi.Atg;

public interface IAtgApi
{
    /// <summary>
    /// Get access token, Obtain access credentials.
    /// It will expire every 10 minutes.
    /// </summary>
    [Get("/token")]
    Task<ApiResponse<BaseResponse<AccessTokenResponse>>> GetAccessTokenAsync(
        [Header("X-Operator")] string xOperator,
        [Header("X-key")] string xKey);
    
    /// <summary>Register new users.</summary>
    [Post("/register")]
    Task<ApiResponse<object>> RegisterAsync(
        [Body] AtgRegisterRequest payload,
        [Header("X-Token")] string token);
    
    /// <summary>Obtain a list of game provider information.</summary>
    /// <param name="token">Generate access credentials.</param>
    /// <param name="page">Page number preset 1.</param>
    /// <param name="pageSize">The preset number of returned records is 10.</param>
    /// <param name="sortBy">The default ID is used for sorting column names.</param>
    /// <param name="sortOrder">The default sorting style is "asc". Allowed values are: "asc" and "desc".</param>
    /// <returns></returns>
    [Get("/game-providers")]
    Task<ApiResponse<BaseResponse<GameProviderResponse>>> GetGameProvidersAsync(
        [Header("X-Token")] string token,
        [Query("page")] int? page = null,
        [Query("rows")] int? pageSize = null,
        [Query("sidx")] string? sortBy = null,
        [Query("sord")] string? sortOrder = null);
    
    /// <summary>Obtain the user's balance with the supplier.</summary>
    [Get("/game-providers/{providerId}/balance")]
    Task<ApiResponse<BaseResponse<GetGameBalanceResponse>>> GetGameBalanceAsync(
        int providerId,
        [Header("X-Token")] string token,
        [Query("username")] string username);
    
    /// <summary>Update user's balance with supplier.</summary>
    [Post("/game-providers/{providerId}/balance")]
    Task<ApiResponse<BaseResponse<GetGameBalanceResponse>>> UpdateGameBalanceAsync(
        int providerId,
        [Header("X-Token")] string token,
        [Body] UpdateGameBalanceRequest payload);

    /// <summary>Get the game list.</summary>
    /// <param name="token">Generate access credentials.</param>
    /// <param name="provider">Game provider ID</param>
    /// <param name="category">Game Categories</param>
    /// <param name="pageSize">The preset number of returned records is 50.</param>
    /// <param name="page">Page number preset 1.</param>
    /// <param name="sortBy">The default ID is used for sorting column names.</param>
    /// <param name="sortOrder">The default sorting style is "asc". Allowed values are: "asc" and "desc".</param>
    /// <param name="local">Filter games in a specified language, supporting zh-cn, zh-tw, en, jp, vn.</param>
    /// <param name="type">Game type preset blank allowed values: "desktop", "mobile".</param>
    /// <returns></returns>
    [Get("/games")]
    Task<ApiResponse<BaseResponse<GetGamesResponse>>> GetGamesAsync(
        [Header("X-Token")] string token,
        [Query("provider")] int provider,
        [Query("category")] string? category = null,
        [Query("rows")] int? pageSize = null,
        [Query("page")] int? page = null,
        [Query("sidx")] string? sortBy = null,
        [Query("sord")] string? sortOrder = null,
        [Query("local")] string? local = null,
        [Query("type")] string? type = null);
    
    /// <summary>Obtain the game key.</summary>
    /// <param name="providerId">Game provider ID.</param>
    /// <param name="token">Generate access credentials.</param>
    /// <param name="gameCode">Game Code Name.</param>
    /// <param name="username">username.</param>
    /// <param name="fullname">The user's displayed name in the game (if not set, it will be displayed as username).</param>
    /// <returns></returns>
    [Get("/game-providers/{providerId}/games/{gameCode}/key")]
    Task<ApiResponse<BaseResponse<GetGameKeyResponse>>> GetGameKeyAsync(
        int providerId,
        [Header("X-Token")] string token,
        string gameCode,
        [Query("username")] string username,
        [Query("fullname")] string? fullname);
    
    /// <summary>Game entry point.</summary>
    /// <param name="providerId">Game provider ID.</param>
    /// <param name="token"></param>
    /// <param name="key">Game Key.</param>
    /// <param name="type">The default values for the generated link type (desktop) are: "desktop", "mobile".</param>
    /// <param name="local">Language preset operator allowed values: "zh-cn", "zh-tw", "en", "vn" (Simplified Chinese, Traditional Chinese, English, Vietnamese).</param>
    /// <returns></returns>
    [Get("/game-providers/{providerId}/play")]
    Task<ApiResponse<BaseResponse<PlayGameResponse>>> PlayGameAsync(
        int providerId,
        [Header("X-Token")] string token,
        [Query("key")] string key,
        [Query("type")] string? type = null,
        [Query("local")] string? local = null);
    
    /// <summary>Get the game lobby link.</summary>
    /// <param name="providerId">Game provider ID.</param>
    /// <param name="token">Generate access credentials</param>
    /// <param name="username">username</param>
    /// <param name="headless">ide table headers? Default: No: 0.</param>
    /// <param name="dark">Dark theme preferred, default is no: 0.</param>
    /// <returns></returns>
    [Get("/game-providers/{providerId}/lobby")]
    Task<ApiResponse<BaseResponse<GetGameLobbyResponse>>> PlayGameLobbyAsync(
        int providerId,
        [Header("X-Token")] string token,
        [Query("username")] string username,
        [Query("headless")] int headless,
        [Query("dark")] int dark);
}