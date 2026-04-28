using ALittleLeaf.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Banner
{
    public class BannerRepository : IBannerRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public BannerRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        // R3: hard-cap at 5 — never sends more than 5 banners to the public homepage
        public Task<List<Models.Banner>> GetActiveBannersAsync() =>
            _context.Banners
                .Where(b => b.IsActive)
                .OrderBy(b => b.DisplayOrder)
                .Take(5)
                .ToListAsync();

        public Task<List<Models.Banner>> GetAllBannersAsync() =>
            _context.Banners
                .OrderBy(b => b.DisplayOrder)
                .ToListAsync();

        public Task<Models.Banner?> GetByIdAsync(int id) =>
            _context.Banners.FirstOrDefaultAsync(b => b.BannerId == id);

        public void Add(Models.Banner banner) =>
            _context.Banners.Add(banner);

        public void Remove(Models.Banner banner) =>
            _context.Banners.Remove(banner);

        public Task SaveChangesAsync() =>
            _context.SaveChangesAsync();
    }
}
