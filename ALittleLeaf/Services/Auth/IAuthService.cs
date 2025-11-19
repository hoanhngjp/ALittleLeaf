using ALittleLeaf.ViewModels;

namespace ALittleLeaf.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthServiceResult> RegisterAsync(RegisterViewModel model);
        Task<AuthServiceResult> LoginAsync(UserLoggedViewModel model);
        Task LogoutAsync();
    }
}
