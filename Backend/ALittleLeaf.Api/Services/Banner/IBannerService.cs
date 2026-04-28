using ALittleLeaf.Api.DTOs.Banner;

namespace ALittleLeaf.Api.Services.Banner
{
    public interface IBannerService
    {
        Task<List<BannerDto>>      GetActiveBannersAsync();
        Task<List<BannerAdminDto>> GetAllBannersAsync();
        Task<BannerAdminDto>       CreateBannerAsync(CreateBannerDto dto);
        Task<BannerAdminDto?>      UpdateBannerAsync(int id, UpdateBannerDto dto);
        Task<bool>                 DeleteBannerAsync(int id);
    }
}
