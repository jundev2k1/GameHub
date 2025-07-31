using game_x.share.Attributes;

namespace game_x.domain.Constants;

public static class MessageCode
{
    public enum System
    {
        #region ■ Message group: Error
        /// <summary>An unexpected server error occurred, possibly due to unhandled exception.</summary>
        [EnumMetadata("An unexpected error occurred. Please try again later.")]
        SystemError = 10000,
        /// <summary>The Server is currently busy due to high traffic or internal overload.</summary>
        [EnumMetadata("The system is busy. Please try again later.")]
        SystemBusy = 10001,
        /// <summary>The System is under maintenance.</summary>
        [EnumMetadata("The service is under maintenance.")]
        SystemMaintenance = 10002,
        /// <summary>The request type or content type is not supported.</summary>
        [EnumMetadata("The request type is not supported.")]
        UnsupportedType = 10003,
        /// <summary>Request parameters are invalid or missing.</summary>
        [EnumMetadata("Invalid or missing parameters.")]
        InvalidParameters = 10004,
        /// <summary>Conflict occurred, such as trying to create a duplicate resource.</summary>
        [EnumMetadata("The resource already exists.")]
        Conflict = 10005,
        /// <summary>Too many requests have been made in a short period of time.</summary>
        [EnumMetadata("Too many requests have been made. Please try again later.")]
        TooManyRequests = 10006,
        /// <summary>Operation is not allowed or violates business logic.</summary>
        [EnumMetadata("The requested operation is invalid.")]
        InvalidOperation = 10007,
        /// <summary>External service or dependency failed.</summary>
        [EnumMetadata("A service error occurred. Please try again later.")]
        DependencyFailure = 10008,
        /// <summary>The Verification code is invalid or has expired.</summary>
        [EnumMetadata("The verification code is invalid or has expired.")]
        InvalidVerifyCode = 10009,
        /// <summary>The request timed out.</summary>
        [EnumMetadata("The request timed out.")]
        RequestTimeout = 10010,
        /// <summary>Uploaded file format is not supported.</summary>
        [EnumMetadata("Unsupported file format.")]
        InvalidFileType = 10011,
        /// <summary>Uploaded file exceeds size limit.</summary>
        [EnumMetadata("The file size exceeds the allowed limit.")]
        FileTooLarge = 10012,
        /// <summary>Failed to generate unique token after multiple attempts.</summary>
        [EnumMetadata("An error occurred while processing your request.")]
        TokenGenerationFailed = 10013,
        /// <summary>The request was canceled by client or server.</summary>
        [EnumMetadata("The request could not be completed.")]
        RequestCancelled = 10014,
        /// <summary>Requested resource does not exist.</summary>
        [EnumMetadata("The requested resource was not found.")]
        ResourceNotFound = 10015,
        /// <summary>Authentication failed or user is not logged in.</summary>
        [EnumMetadata("You must be logged in to continue.")]
        Unauthorized = 10016,
        /// <summary>Access to the resource is forbidden.</summary>
        [EnumMetadata("You do not have permission to access this resource.")]
        Forbidden = 10017,
        /// <summary>One or more validation rules failed.</summary>
        [EnumMetadata("One or more validation failures occurred.")]
        ValidateFailed = 10018,
        /// <summary>The requested resource has already been deleted or is no longer available.</summary>
        [EnumMetadata("This item has already been deleted or is no longer available.")]
        ItemAlreadyDeleted = 10019,
        /// <summary>The request was already processed or is no longer valid.</summary>
        [EnumMetadata("This request has already been handled.")]
        RequestAlreadyHandled = 10020,
        /// <summary>The current state of the resource does not allow this action.</summary>
        [EnumMetadata("The resource is in an invalid state for this action.")]
        InvalidResourceState = 10021,
        /// <summary>The specified name already exists.</summary>
        [EnumMetadata("The name you entered is already in use.")]
        NameAlreadyExists = 10022,
        /// <summary>A resource with the same value already exists.</summary>
        [EnumMetadata("A resource with the same value already exists.")]
        DuplicateValue = 10023,
        #endregion

        #region ■ Message group: Success
        /// <summary>Generic success message.</summary>
        [EnumMetadata("Request succeeded.")]
        Success = 10050,
        /// <summary>Resource was successfully created.</summary>
        [EnumMetadata("Resource created successfully.")]
        Created = 10051,
        /// <summary>Resource was successfully updated.</summary>
        [EnumMetadata("Resource updated successfully.")]
        Updated = 10052,
        /// <summary>The Resource was successfully deleted.</summary>
        [EnumMetadata("Resource deleted successfully.")]
        Deleted = 10053,
        /// <summary>The Request was successfully processed, no content returned.</summary>
        [EnumMetadata("Request completed successfully.")]
        NoContent = 10054,
        /// <summary>Action was accepted and will be processed asynchronously.</summary>
        [EnumMetadata("Request accepted for processing.")]
        Accepted = 10055,
        /// <summary>Indicates that the system configuration has been refreshed successfully.</summary>
        [EnumMetadata("Refresh completed successfully.")]
        RefreshSuccess = 10056,
        /// <summary>Indicates that the user has logged in successfully. Typically used after successful authentication.</summary>
        [EnumMetadata("Login completed successfully.")]
        LoginSuccess = 10057,
        /// <summary>Indicates that the user has logged out successfully. Typically used when a user ends their session.</summary>
        [EnumMetadata("Logout completed successfully.")]
        LogoutSuccess = 10058,
        /// <summary>Indicates that the authentication or validation was successful. Typically used for non-login validation scenarios such as QR code scans, OTPs, or access tokens.</summary>
        [EnumMetadata("Validation completed successfully.")]
        ValidationSuccess = 10059,
        /// <summary>Indicates that the email has been sent successfully. Typically used after triggering a mail-sending operation to an external SMTP or provider.</summary>
        [EnumMetadata("Email sent successfully.")]
        EmailSendSuccess = 10060,
        /// <summary>
        ///     Indicates that the image was uploaded successfully.
        ///     Typically used after uploading avatars, product images, or other media files.
        /// </summary>
        [EnumMetadata("Image uploaded successfully.")]
        ImageUploadSuccess = 10061,
        #endregion 
    }

    public enum User
    {
        #region ■ Message group: Error
        /// <summary>The account has been locked, possibly due to multiple failed login attempts or other security reasons.</summary>
        [EnumMetadata("Your account has been locked. Please try again later.")]
        UserLocked = 10100,
        /// <summary>The account is not allowed to sign in, possibly due to being unverified or other restrictions.</summary>
        [EnumMetadata("This account is not allowed to sign in.")]
        UserNotAllowed = 10101,
        /// <summary>Two-factor authentication is required, usually for enhanced security.</summary>
        [EnumMetadata("Two-factor authentication is required.")]
        UserRequiresTwoFactor = 10102,
        /// <summary>Invalid username or password. Typically used for login failures.</summary>
        [EnumMetadata("Incorrect email or password.")]
        UserInvalidCredentials = 10103,
        /// <summary>The Email has not been confirmed. Usually used in post-registration verification processes.</summary>
        [EnumMetadata("The email address is not confirmed.")]
        UserNotConfirmed = 10104,
        /// <summary>The account has been disabled, either by the user or by an administrator.</summary>
        [EnumMetadata("The account is disabled.")]
        UserDisabled = 10105,
        /// <summary>The email address is already verified.</summary>
        [EnumMetadata("The email address has already been verified.")]
        EmailAlreadyVerified = 10106,
        /// <summary>The user was not found. Typically used during login or when querying user data.</summary>
        [EnumMetadata("User not found.")]
        UserNotFound = 10107,
        /// <summary>The user already exists. Typically used to check for duplicates during registration.</summary>
        [EnumMetadata("User already exists.")]
        UserAlreadyExists = 10108,
        /// <summary>The user is invalid, possibly due to incomplete or malformed data.</summary>
        [EnumMetadata("The account is invalid.")]
        UserInvalid = 10109,
        /// <summary>Password change failed. This may be due to invalid current password, weak new password, or server-side error.</summary>
        [EnumMetadata("Failed to change password.")]
        UserChangePasswordFail = 10110,
        /// <summary>This email is already registered.</summary>
        [EnumMetadata("The email address is already in use.")]
        EmailAlreadyExists = 10111,
        /// <summary>The phone number is existed.</summary>
        [EnumMetadata("The phone number is already in use.")]
        PhoneAlreadyExists = 10112,
        /// <summary>The nickname is existed.</summary>
        [EnumMetadata("The nickname is already in use.")]
        NicknameAlreadyExists = 10113,
        /// <summary>Too many failed attempts. The verification process is temporarily locked.</summary>
        [EnumMetadata("Too many failed attempts. Please try again later.")]
        VerifyTooManyFailedAttempts = 10114,
        /// <summary>You must wait before requesting a new verification code.</summary>
        [EnumMetadata("Please wait before requesting a new verification code.")]
        VerifyResendCooldown = 10115,
        /// <summary>The current KYC status does not allow this action.</summary>
        [EnumMetadata("KYC status is invalid for this action.")]
        KycInvalidStatus = 10116,
        #endregion

        #region ■ Message group: Success
        /// <summary>User registration completed successfully. Typically used after the user has registered and any post-registration actions (such as sending verification email) have been triggered.</summary>
        [EnumMetadata("User registered successfully.")]
        UserRegisterSuccess = 10150,
        /// <summary>The user's password has been changed successfully. Typically used after completing a password update or reset process.</summary>
        [EnumMetadata("Password changed successfully.")]
        UserChangePasswordSuccess = 10151,
        /// <summary>The user's email has been verified successfully. Typically used after completing an email verify process.</summary>
        [EnumMetadata("Email verified successfully.")]
        EmailVerifySuccess = 10152,
        #endregion
    }
}
