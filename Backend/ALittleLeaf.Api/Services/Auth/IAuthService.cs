using ALittleLeaf.Api.DTOs.Auth;

namespace ALittleLeaf.Api.Services.Auth
{
    public interface IAuthService
    {
        /// <summary>Validate credentials, generate JWT + refresh token.</summary>
        Task<AuthServiceResult> LoginAsync(LoginRequestDto dto);

        /// <summary>Create user + default address, then return JWT + refresh token.</summary>
        Task<AuthServiceResult> RegisterAsync(RegisterRequestDto dto);

        /// <summary>Validate the refresh token and issue a new JWT pair.</summary>
        Task<AuthServiceResult> RefreshTokenAsync(RefreshTokenRequestDto dto);

        /// <summary>Revoke the supplied refresh token in the database.</summary>
        Task<bool> LogoutAsync(string refreshToken);

        /// <summary>Verify a Google ID Token and return a JWT pair. Creates or links the account.</summary>
        Task<AuthServiceResult> GoogleLoginAsync(string idToken);
    }
}
