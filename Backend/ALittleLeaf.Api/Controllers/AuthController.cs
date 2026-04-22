using ALittleLeaf.Api.DTOs.Auth;
using ALittleLeaf.Api.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ALittleLeaf.Api.Controllers
{
    /// <summary>
    /// Handles authentication: login, registration, token refresh, and logout.
    /// All business logic is delegated to IAuthService.
    /// </summary>
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // ── POST /api/auth/login ──────────────────────────────────────────────

        /// <summary>
        /// Authenticates a user and returns a JWT access token + refresh token.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.LoginAsync(dto);

            if (!result.Succeeded)
                return Unauthorized(new { error = result.ErrorMessage });

            return Ok(BuildLoginResponse(result));
        }

        // ── POST /api/auth/register ───────────────────────────────────────────

        /// <summary>
        /// Creates a new customer account and returns a JWT access token + refresh token.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(dto);

            if (!result.Succeeded)
            {
                // Email already taken → 409 Conflict
                if (result.ErrorMessage!.Contains("đã tồn tại"))
                    return Conflict(new { error = result.ErrorMessage });

                return BadRequest(new { error = result.ErrorMessage });
            }

            return StatusCode(StatusCodes.Status201Created, BuildLoginResponse(result));
        }

        // ── POST /api/auth/refresh-token ──────────────────────────────────────

        /// <summary>
        /// Exchanges a valid refresh token for a new JWT access token + refresh token pair.
        /// </summary>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RefreshTokenAsync(dto);

            if (!result.Succeeded)
                return Unauthorized(new { error = result.ErrorMessage });

            return Ok(BuildLoginResponse(result));
        }

        // ── POST /api/auth/logout ─────────────────────────────────────────────

        /// <summary>
        /// Revokes the supplied refresh token. The client must discard its access token locally.
        /// Requires a valid JWT (caller must be logged in).
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authService.LogoutAsync(dto.RefreshToken);

            return Ok(new { message = "Bạn đã đăng xuất thành công." });
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static LoginResponseDto BuildLoginResponse(AuthServiceResult result) =>
            new()
            {
                AccessToken  = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                User = new UserDto
                {
                    UserId   = result.User!.UserId,
                    Email    = result.User.UserEmail,
                    FullName = result.User.UserFullname,
                    Role     = result.User.UserRole
                }
            };
    }
}
