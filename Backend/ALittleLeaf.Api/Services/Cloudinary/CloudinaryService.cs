using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace ALittleLeaf.Api.Services.Cloudinary
{
    /// <summary>
    /// Cloudinary image storage service.
    /// Credentials are read from IConfiguration (env-driven via .env or appsettings).
    /// </summary>
    public class CloudinaryService : ICloudinaryService
    {
        private readonly CloudinaryDotNet.Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;

        public CloudinaryService(IConfiguration configuration, ILogger<CloudinaryService> logger)
        {
            _logger = logger;

            var cloudName = Environment.GetEnvironmentVariable("CLOUDINARY_CLOUD_NAME")
                ?? configuration["Cloudinary:CloudName"]
                ?? throw new InvalidOperationException("Cloudinary:CloudName is not configured.");

            var apiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY")
                ?? configuration["Cloudinary:ApiKey"]
                ?? throw new InvalidOperationException("Cloudinary:ApiKey is not configured.");

            var apiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET")
                ?? configuration["Cloudinary:ApiSecret"]
                ?? throw new InvalidOperationException("Cloudinary:ApiSecret is not configured.");

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new CloudinaryDotNet.Cloudinary(account) { Api = { Secure = true } };
        }

        /// <inheritdoc />
        public async Task<string> UploadImageAsync(IFormFile file, string? folder = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.", nameof(file));

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder ?? "alittleleaf",
                Overwrite = false,
                UniqueFilename = true,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                _logger.LogError("Cloudinary upload failed: {Error}", result.Error.Message);
                throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
            }

            _logger.LogInformation("Cloudinary upload succeeded. PublicId={PublicId} Url={Url}",
                result.PublicId, result.SecureUrl);

            return result.SecureUrl.ToString();
        }

        /// <inheritdoc />
        public async Task<(string Url, string PublicId)> UploadImageWithPublicIdAsync(IFormFile file, string? folder = null)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.", nameof(file));

            await using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = folder ?? "alittleleaf",
                Overwrite = false,
                UniqueFilename = true,
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
            {
                _logger.LogError("Cloudinary upload failed: {Error}", result.Error.Message);
                throw new InvalidOperationException($"Cloudinary upload failed: {result.Error.Message}");
            }

            _logger.LogInformation("Cloudinary upload succeeded. PublicId={PublicId} Url={Url}",
                result.PublicId, result.SecureUrl);

            return (result.SecureUrl.ToString(), result.PublicId);
        }

        /// <inheritdoc />
        public async Task<bool> DeleteImageAsync(string publicId)
        {
            if (string.IsNullOrWhiteSpace(publicId))
                throw new ArgumentException("PublicId must not be empty.", nameof(publicId));

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                _logger.LogError("Cloudinary deletion failed for PublicId={PublicId}: {Error}",
                    publicId, result.Error.Message);
                return false;
            }

            _logger.LogInformation("Cloudinary deletion succeeded. PublicId={PublicId} Result={Result}",
                publicId, result.Result);

            return result.Result == "ok";
        }
    }
}
