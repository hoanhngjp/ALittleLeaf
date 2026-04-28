using ALittleLeaf.Api.DTOs.Banner;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Banner;
using ALittleLeaf.Api.Services.Banner;
using ALittleLeaf.Api.Services.Cloudinary;
using Microsoft.AspNetCore.Http;
using Moq;

namespace ALittleLeaf.Tests;

public class BannerServiceTests
{
    // ── Fixture helpers ───────────────────────────────────────────────────────

    private static (BannerService svc,
                    Mock<IBannerRepository> repoMock,
                    Mock<ICloudinaryService> cloudinaryMock)
        BuildSut()
    {
        var repoMock       = new Mock<IBannerRepository>();
        var cloudinaryMock = new Mock<ICloudinaryService>();
        var svc            = new BannerService(repoMock.Object, cloudinaryMock.Object);
        return (svc, repoMock, cloudinaryMock);
    }

    /// <summary>
    /// Creates a mock IFormFile with configurable ContentType and Length.
    /// OpenReadStream returns an empty stream — sufficient for validation tests.
    /// </summary>
    private static IFormFile MockFile(string contentType, long sizeBytes)
    {
        var mock = new Mock<IFormFile>();
        mock.Setup(f => f.ContentType).Returns(contentType);
        mock.Setup(f => f.Length).Returns(sizeBytes);
        mock.Setup(f => f.FileName).Returns("test.jpg");
        mock.Setup(f => f.OpenReadStream()).Returns(new MemoryStream(new byte[sizeBytes > 0 ? 1 : 0]));
        return mock.Object;
    }

    // ── Scenario 1: File exceeds 5 MB ────────────────────────────────────────

    [Fact]
    public async Task CreateBannerAsync_FileTooLarge_ThrowsArgumentException()
    {
        var (svc, _, _) = BuildSut();

        long sixMegabytes = 6L * 1024 * 1024;
        var dto = new CreateBannerDto
        {
            ImageFile    = MockFile("image/jpeg", sixMegabytes),
            DisplayOrder = 0,
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => svc.CreateBannerAsync(dto));

        Assert.Contains("5 MB", ex.Message);
    }

    // ── Scenario 2: Invalid MIME type ────────────────────────────────────────

    [Fact]
    public async Task CreateBannerAsync_InvalidMimeType_ThrowsArgumentException()
    {
        var (svc, _, _) = BuildSut();

        var dto = new CreateBannerDto
        {
            ImageFile    = MockFile("application/pdf", 1024),
            DisplayOrder = 0,
        };

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => svc.CreateBannerAsync(dto));

        Assert.Contains("image/jpeg", ex.Message);
        Assert.Contains("image/png",  ex.Message);
        Assert.Contains("image/webp", ex.Message);
    }

    // ── Scenario 3: R2 — f_auto,q_auto injected into stored URL ─────────────

    [Fact]
    public async Task CreateBannerAsync_ValidFile_InjectsOptimisationTransformInStoredUrl()
    {
        var (svc, repoMock, cloudinaryMock) = BuildSut();

        const string rawUrl   = "https://res.cloudinary.com/demo/image/upload/v1234/sample.jpg";
        const string publicId = "sample";

        cloudinaryMock
            .Setup(c => c.UploadImageWithPublicIdAsync(It.IsAny<IFormFile>(), "banners"))
            .ReturnsAsync((rawUrl, publicId));

        // Capture the Banner object passed to Add so we can inspect the stored URL
        Banner? capturedBanner = null;
        repoMock
            .Setup(r => r.Add(It.IsAny<Banner>()))
            .Callback<Banner>(b => capturedBanner = b);

        repoMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var dto = new CreateBannerDto
        {
            ImageFile    = MockFile("image/jpeg", 1024),
            DisplayOrder = 1,
        };

        await svc.CreateBannerAsync(dto);

        Assert.NotNull(capturedBanner);

        // The stored URL must contain the transformation segment
        Assert.Contains("f_auto,q_auto", capturedBanner!.ImageUrl);

        // The transformation must sit directly after /upload/
        const string expected = "/upload/f_auto,q_auto/v1234/sample.jpg";
        Assert.Contains(expected, capturedBanner.ImageUrl);

        // The raw URL (without the transform) must NOT be stored as-is
        Assert.NotEqual(rawUrl, capturedBanner.ImageUrl);

        // PublicId must be persisted exactly as returned by Cloudinary
        Assert.Equal(publicId, capturedBanner.PublicId);
    }

    // ── Scenario 4: DeleteBannerAsync calls Cloudinary then repo ─────────────

    [Fact]
    public async Task DeleteBannerAsync_ExistingBanner_CallsCloudinaryThenRemovesFromRepo()
    {
        var (svc, repoMock, cloudinaryMock) = BuildSut();

        var banner = new Banner
        {
            BannerId     = 7,
            ImageUrl     = "https://res.cloudinary.com/demo/image/upload/f_auto,q_auto/v1/foo.webp",
            PublicId     = "foo",
            DisplayOrder = 1,
            IsActive     = true,
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow,
        };

        repoMock
            .Setup(r => r.GetByIdAsync(7))
            .ReturnsAsync(banner);

        cloudinaryMock
            .Setup(c => c.DeleteImageAsync("foo"))
            .ReturnsAsync(true);

        repoMock
            .Setup(r => r.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        var result = await svc.DeleteBannerAsync(7);

        Assert.True(result);

        // Cloudinary deletion must have been called with the correct PublicId
        cloudinaryMock.Verify(c => c.DeleteImageAsync("foo"), Times.Once);

        // Repo Remove must have been called with the correct entity
        repoMock.Verify(r => r.Remove(banner), Times.Once);

        // Changes must have been persisted
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // ── Bonus: DeleteBannerAsync returns false for unknown ID ─────────────────

    [Fact]
    public async Task DeleteBannerAsync_NotFound_ReturnsFalseWithoutCallingCloudinary()
    {
        var (svc, repoMock, cloudinaryMock) = BuildSut();

        repoMock
            .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Banner?)null);

        var result = await svc.DeleteBannerAsync(999);

        Assert.False(result);
        cloudinaryMock.Verify(c => c.DeleteImageAsync(It.IsAny<string>()), Times.Never);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
