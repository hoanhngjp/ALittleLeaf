using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Banner
{
    public interface IBannerRepository
    {
        /// <summary>Active banners only, ordered by DisplayOrder, capped at 5 (R3).</summary>
        Task<List<Models.Banner>> GetActiveBannersAsync();

        Task<List<Models.Banner>> GetAllBannersAsync();

        Task<Models.Banner?> GetByIdAsync(int id);

        void Add(Models.Banner banner);

        void Remove(Models.Banner banner);

        Task SaveChangesAsync();
    }
}
