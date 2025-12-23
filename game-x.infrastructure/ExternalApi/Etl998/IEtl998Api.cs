using game_x.share.ExternalApi.Etl998.Dtos;
using game_x.share.ExternalApi.Etl998.Dtos.AccountBalance;
using game_x.share.ExternalApi.Etl998.Dtos.CancelTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.ChangePassword;
using game_x.share.ExternalApi.Etl998.Dtos.ConfirmTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.CreateAccount;
using game_x.share.ExternalApi.Etl998.Dtos.ForwardGame;
using game_x.share.ExternalApi.Etl998.Dtos.IsAccountExist;
using game_x.share.ExternalApi.Etl998.Dtos.ModifyBettingLimit;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
using game_x.share.ExternalApi.Etl998.Dtos.SearchRecord;
using game_x.share.ExternalApi.Etl998.Dtos.SearchTransfer;
using Refit;

namespace game_x.infrastructure.ExternalApi.Etl998;

public interface IEtl998Api
{
    /// <summary>Create Account API</summary>
    [Post("/CreateAccout")]
    Task<ApiResponse<BaseResponse>> CreateAccountAsync([Body] CreateAccountRequest req);
    
    /// <summary>Check Account Existence API</summary>
    [Post("/IsAccountExist")]
    Task<ApiResponse<BaseResponse>> IsAccountExistAsync([Body] IsAccountExistRequest req);
    
    /// <summary>Get Account Balance API</summary>
    [Post("/AccoutBalance")]
    Task<ApiResponse<BaseResponse>> GetAccountBalanceAsync([Body] AccountBalanceRequest req);
    
    /// <summary>Prepare Transfer API</summary>
    [Post("/PrepareTransfer")]
    Task<ApiResponse<BaseResponse>> PrepareTransferAsync([Body] PrepareTransferRequest req);
    
    /// <summary>Confirm Transfer API</summary>
    [Post("/ConfirmTransfer")]
    Task<ApiResponse<BaseResponse>> ConfirmTransferAsync([Body] ConfirmTransferRequest req);
    
    /// <summary>Confirm Transfer API</summary>
    [Post("/SearchTransfer")]
    Task<ApiResponse<BaseResponse>> SearchTransferAsync([Body] SearchTransferRequest req);
    
    /// <summary>Cancel Transfer API</summary>
    [Post("/CancelTransfer")]
    Task<ApiResponse<BaseResponse>> CancelTransferAsync([Body] CancelTransferRequest req);
    
    /// <summary>Forward Game API</summary>
    [Post("/ForwardGame")]
    Task<ApiResponse<BaseResponse>> ForwardGameAsync([Body] ForwardGameRequest req);
    
    /// <summary>Search Records API</summary>
    [Post("/SearchRecords")]
    Task<ApiResponse<BaseResponse>> SearchRecordAsync([Body] SearchRecordRequest req);
    
    /// <summary>Change Password API</summary>
    [Post("/ChangePassWord")]
    Task<ApiResponse<BaseResponse>> ChangePasswordAsync([Body] ChangePasswordRequest req);
    
    /// <summary>Modify Betting Limits API</summary>
    [Post("/EditAccoutTables")]
    Task<ApiResponse<BaseResponse>> ModifyBettingLimitsAsync([Body] ModifyBettingLimitRequest req);
}