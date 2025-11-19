using ALittleLeaf.Models;
using ALittleLeaf.Repository;
using ALittleLeaf.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace ALittleLeaf.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly AlittleLeafDecorContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly PasswordHasher<string> _passwordHasher;

        public AuthService(AlittleLeafDecorContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = new PasswordHasher<string>();
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        public async Task<AuthServiceResult> LoginAsync(UserLoggedViewModel model)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == model.UserEmail);

            if (user == null)
            {
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Thông tin đăng nhập không hợp lệ." };
            }

            var result = _passwordHasher.VerifyHashedPassword(null, user.UserPassword, model.UserPassword);
            bool isPasswordCorrect = result == PasswordVerificationResult.Success;

            if (!isPasswordCorrect)
            {
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Thông tin đăng nhập không hợp lệ." };
            }

            if (!user.UserIsActive)
            {
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Tài khoản của bạn đã bị khóa." };
            }

            // Đăng nhập thành công -> Set Session
            Session.SetString("UserEmail", user.UserEmail);
            Session.SetString("UserFullname", user.UserFullname);
            Session.SetString("UserId", user.UserId.ToString());

            return new AuthServiceResult { Succeeded = true, User = user };
        }

        public async Task<AuthServiceResult> RegisterAsync(RegisterViewModel model)
        {
            // 1. Kiểm tra Email
            if (await _context.Users.AnyAsync(u => u.UserEmail == model.UserEmail))
            {
                return new AuthServiceResult { Succeeded = false, ErrorMessage = "Email đã tồn tại." };
            }

            // 2. Hash mật khẩu
            string hashedPass = _passwordHasher.HashPassword(null, model.UserPassword);

            // 3. Bắt đầu Transactio (Rất quan trọng!)
            // Đảm bảo việc tạo User VÀ Address cùng thành công, hoặc cùng thất bại
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 4. Tạo User
                    var user = new User
                    {
                        UserEmail = model.UserEmail,
                        UserPassword = hashedPass,
                        UserFullname = model.UserFullname,
                        UserSex = model.UserSex,
                        UserBirthday = model.UserBirthday,
                        UserIsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        UserRole = model.UserRole // Thường nên là "customer"
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync(); // Lưu để lấy UserId

                    // 5. Tạo AddressList
                    var address = new AddressList
                    {
                        IdUser = user.UserId,
                        AdrsFullname = user.UserFullname,
                        AdrsAddress = model.Address,
                        AdrsPhone = "N/A", // Sẽ cập nhật sau
                        AdrsIsDefault = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    _context.AddressLists.Add(address);
                    await _context.SaveChangesAsync();

                    // 6. Mọi thứ OK -> Commit
                    await transaction.CommitAsync();

                    // 7. Tự động đăng nhập
                    Session.SetString("UserEmail", user.UserEmail);
                    Session.SetString("UserFullname", user.UserFullname);
                    Session.SetString("UserId", user.UserId.ToString());

                    return new AuthServiceResult { Succeeded = true, User = user };
                }
                catch (Exception ex)
                {
                    // 8. Nếu có lỗi (ví dụ DB sập), Rollback
                    await transaction.RollbackAsync();
                    // Log lỗi ex.Message
                    return new AuthServiceResult { Succeeded = false, ErrorMessage = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại." };
                }
            }
        }

        public async Task LogoutAsync()
        {
            Session.Clear();
            await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
