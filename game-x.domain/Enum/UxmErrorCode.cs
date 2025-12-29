using game_x.share.Attributes;

namespace game_x.domain.Enum;

public enum UxmErrorCode
{
    [EnumMetadata("Unexpected error – An unknown system error occurred. Please try again later or contact technical support.")]
    UnexpectedError = 10000,
    [EnumMetadata("System busy – Please try again later.")]
    SystemBusy = 10001,
    [EnumMetadata("Invalid merchant – The specified merchant is invalid.")]
    InvalidMerchant = 20201,
    [EnumMetadata("Insufficient merchant balance – The merchant's balance is insufficient.")]
    InsufficientMerchantBalance = 20202,
    [EnumMetadata("Incomplete merchant configuration.")]
    IncompleteMerchantConfig = 20203,
    [EnumMetadata("Duplicate order – The submitted order already exists.")]
    DuplicateOrder = 20304,
    [EnumMetadata("Maximum number of pending orders reached.")]
    MaxPendingOrders = 20310,
    [EnumMetadata("Invalid address format.")]
    InvalidAddress = 20402,
    [EnumMetadata("Insufficient wallet balance – The wallet resources are insufficient.")]
    InsufficientWalletBalance = 20404,
    [EnumMetadata("Invalid signature.")]
    InvalidSignature = 50001,
    [EnumMetadata("Invalid public key.")]
    InvalidPublicKey = 50002,
    [EnumMetadata("Invalid data format.")]
    InvalidDataFormat = 50003
}