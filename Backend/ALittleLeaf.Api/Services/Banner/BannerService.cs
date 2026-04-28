using ALittleLeaf.Api.DTOs.Banner;
using ALittleLeaf.Api.Repositories.Banner;
using ALittleLeaf.Api.Services.Cloudinary;

namespace ALittleLeaf.Api.Services.Banner
{
    public class BannerService : IBannerService
    {
        private static readonly HashSet<string> _allowedTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        private readonly IBannerRepository  _repo;
        private readonly ICloudinaryService _cloudinary;

        public BannerService(IBannerRepository repo, ICloudinaryService cloudinary)
        {
            _repo       = repo;
            _cloudinary = cloudinary;
        }

        public async Task<List<BannerDto>> GetActiveBannersAsync()
        {
            var banners = await _repo.GetActiveBannersAsync();
            return banners.Select(ToDto).ToList();
        }

        public async Task<List<BannerAdminDto>> GetAllBannersAsync()
        {
            var banners = await _repo.GetAllBannersAsync();
            return banners.Select(ToAdminDto).ToList();
        }

        public async Task<BannerAdminDto> CreateBannerAsync(CreateBannerDto dto)
        {
            // R1 — file validation
            if (dto.ImageFile == null || dto.ImageFile.Length == 0)
                throw new ArgumentException("An image file is required.");

            if (dto.ImageFile.Length > MaxFileSizeBytes)
                throw new ArgumentException("File size must not exceed 5 MB.");

            if (!_allowedTypes.Contains(dto.ImageFile.ContentType))
                throw new ArgumentException(
                    "Unsupported file type. Allowed types: image/jpeg, image/png, image/webp.");

            // Upload — returns both URL and PublicId
            var (rawUrl, publicId) = await _cloudinary.UploadImageWithPublicIdAsync(dto.ImageFile, "banners");

            // R2 — insert f_auto,q_auto transformation into the Cloudinary URL
            var optimisedUrl = InjectCloudinaryTransform(rawUrl, "f_auto,q_auto");

            var banner = new Models.Banner
            {
                ImageUrl     = optimisedUrl,
                PublicId     = publicId,
                TargetUrl    = dto.TargetUrl,
                DisplayOrder = dto.DisplayOrder,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow,
                UpdatedAt    = DateTime.UtcNow
            };

            _repo.Add(banner);
            await _repo.SaveChangesAsync();

            return ToAdminDto(banner);
        }

        public async Task<BannerAdminDto?> UpdateBannerAsync(int id, UpdateBannerDto dto)
        {
            var banner = await _repo.GetByIdAsync(id);
            if (banner is null) return null;

            if (dto.IsActive.HasValue)     banner.IsActive     = dto.IsActive.Value;
            if (dto.DisplayOrder.HasValue) banner.DisplayOrder = dto.DisplayOrder.Value;
            if (dto.TargetUrl is not null) banner.TargetUrl    = dto.TargetUrl;

            banner.UpdatedAt = DateTime.UtcNow;
            await _repo.SaveChangesAsync();

            return ToAdminDto(banner);
        }

        public async Task<bool> DeleteBannerAsync(int id)
        {
            var banner = await _repo.GetByIdAsync(id);
            if (banner is null) return false;

            // Delete from Cloudinary first — if this fails we keep the DB record intact
            await _cloudinary.DeleteImageAsync(banner.PublicId);

            _repo.Remove(banner);
            await _repo.SaveChangesAsync();

            return true;
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Inserts a Cloudinary transformation segment immediately after "/upload/".
        /// e.g. ".../upload/v123/foo.webp" → ".../upload/f_auto,q_auto/v123/foo.webp"
        /// If "/upload/" is not present the URL is returned unchanged.
        /// </summary>
        private static string InjectCloudinaryTransform(string url, string transform)
        {
            const string marker = "/upload/";
            var idx = url.IndexOf(marker, StringComparison.Ordinal);
            if (idx < 0) return url;

            return url.Insert(idx + marker.Length, transform + "/");
        }

        private static BannerDto ToDto(Models.Banner b) => new()
        {
            BannerId     = b.BannerId,
            ImageUrl     = b.ImageUrl,
            TargetUrl    = b.TargetUrl,
            DisplayOrder = b.DisplayOrder
        };

        private static BannerAdminDto ToAdminDto(Models.Banner b) => new()
        {
            BannerId     = b.BannerId,
            ImageUrl     = b.ImageUrl,
            PublicId     = b.PublicId,
            TargetUrl    = b.TargetUrl,
            DisplayOrder = b.DisplayOrder,
            IsActive     = b.IsActive,
            CreatedAt    = b.CreatedAt,
            UpdatedAt    = b.UpdatedAt
        };
    }
}
