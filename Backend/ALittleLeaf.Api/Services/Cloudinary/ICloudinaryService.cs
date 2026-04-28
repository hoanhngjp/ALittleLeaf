namespace ALittleLeaf.Api.Services.Cloudinary
{
    /// <summary>
    /// Abstraction for Cloudinary image storage operations.
    /// </summary>
    public interface ICloudinaryService
    {
        /// <summary>
        /// Uploads an image file to Cloudinary and returns the secure HTTPS URL.
        /// </summary>
        /// <param name="file">The image file to upload.</param>
        /// <param name="folder">Optional Cloudinary folder (e.g. "products", "categories").</param>
        /// <returns>The secure Cloudinary URL of the uploaded image.</returns>
        Task<string> UploadImageAsync(IFormFile file, string? folder = null);

        /// <summary>
        /// Deletes an image from Cloudinary by its public ID.
        /// </summary>
        /// <param name="publicId">The Cloudinary public ID of the image to delete.</param>
        /// <returns>True if deletion succeeded, false otherwise.</returns>
        Task<bool> DeleteImageAsync(string publicId);

        /// <summary>
        /// Uploads an image and returns both the secure URL and the Cloudinary public_id.
        /// </summary>
        Task<(string Url, string PublicId)> UploadImageWithPublicIdAsync(IFormFile file, string? folder = null);
    }
}
