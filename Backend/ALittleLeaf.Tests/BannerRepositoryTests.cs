using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Banner;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Tests;

public class BannerRepositoryTests
{
    // ── Fixture helpers ───────────────────────────────────────────────────────

    /// <summary>
    /// Creates a fresh isolated in-memory database for each test so tests
    /// cannot share or corrupt each other's data.
    /// </summary>
    private static AlittleLeafDecorContext BuildDb(string dbName)
    {
        var opts = new DbContextOptionsBuilder<AlittleLeafDecorContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AlittleLeafDecorContext(opts);
    }

    /// <summary>
    /// Builds a Banner with sensible defaults; caller overrides only what matters.
    /// </summary>
    private static Banner MakeBanner(int id, int displayOrder, bool isActive) => new()
    {
        BannerId     = id,
        ImageUrl     = $"https://res.cloudinary.com/demo/image/upload/f_auto,q_auto/v1/banner_{id}.webp",
        PublicId     = $"banner_{id}",
        DisplayOrder = displayOrder,
        IsActive     = isActive,
        CreatedAt    = DateTime.UtcNow,
        UpdatedAt    = DateTime.UtcNow,
    };

    // ── Scenario 5: R3 — Take(5), inactive filtered, ordered by DisplayOrder ─

    [Fact]
    public async Task GetActiveBannersAsync_ReturnsAtMost5_FiltersInactive_OrderedByDisplayOrder()
    {
        await using var db = BuildDb(nameof(GetActiveBannersAsync_ReturnsAtMost5_FiltersInactive_OrderedByDisplayOrder));

        // Seed: 7 active banners with scrambled DisplayOrder values + 3 inactive banners
        // Active banners (DisplayOrder deliberately out of sequence)
        db.Banners.AddRange(
            MakeBanner(id: 1,  displayOrder: 50, isActive: true),
            MakeBanner(id: 2,  displayOrder: 10, isActive: true),
            MakeBanner(id: 3,  displayOrder: 30, isActive: true),
            MakeBanner(id: 4,  displayOrder: 20, isActive: true),
            MakeBanner(id: 5,  displayOrder: 40, isActive: true),
            MakeBanner(id: 6,  displayOrder: 70, isActive: true),
            MakeBanner(id: 7,  displayOrder: 60, isActive: true),
            // Inactive — must never appear in the result
            MakeBanner(id: 8,  displayOrder:  1, isActive: false),
            MakeBanner(id: 9,  displayOrder:  2, isActive: false),
            MakeBanner(id: 10, displayOrder:  3, isActive: false)
        );
        await db.SaveChangesAsync();

        var repo   = new BannerRepository(db);
        var result = await repo.GetActiveBannersAsync();

        // R3: hard cap at 5 — never more, regardless of how many are active
        Assert.Equal(5, result.Count);

        // All returned banners must be active
        Assert.All(result, b => Assert.True(b.IsActive));

        // Must be ordered by DisplayOrder ascending
        // With 7 active banners ordered [10, 20, 30, 40, 50, 60, 70], Take(5) yields [10, 20, 30, 40, 50]
        var expectedOrder = new[] { 10, 20, 30, 40, 50 };
        Assert.Equal(expectedOrder, result.Select(b => b.DisplayOrder));

        // None of the inactive banners (ids 8, 9, 10) must appear
        var returnedIds = result.Select(b => b.BannerId).ToHashSet();
        Assert.DoesNotContain(8,  returnedIds);
        Assert.DoesNotContain(9,  returnedIds);
        Assert.DoesNotContain(10, returnedIds);
    }

    // ── Edge case: fewer than 5 active banners ────────────────────────────────

    [Fact]
    public async Task GetActiveBannersAsync_FewerThan5Active_ReturnsOnlyActiveOnes()
    {
        await using var db = BuildDb(nameof(GetActiveBannersAsync_FewerThan5Active_ReturnsOnlyActiveOnes));

        db.Banners.AddRange(
            MakeBanner(id: 1, displayOrder: 2, isActive: true),
            MakeBanner(id: 2, displayOrder: 1, isActive: true),
            MakeBanner(id: 3, displayOrder: 3, isActive: false)
        );
        await db.SaveChangesAsync();

        var repo   = new BannerRepository(db);
        var result = await repo.GetActiveBannersAsync();

        Assert.Equal(2, result.Count);
        Assert.All(result, b => Assert.True(b.IsActive));
        // Ordered: DisplayOrder 1 first, then 2
        Assert.Equal(new[] { 1, 2 }, result.Select(b => b.DisplayOrder));
    }

    // ── Edge case: no active banners at all ───────────────────────────────────

    [Fact]
    public async Task GetActiveBannersAsync_NoActiveBanners_ReturnsEmptyList()
    {
        await using var db = BuildDb(nameof(GetActiveBannersAsync_NoActiveBanners_ReturnsEmptyList));

        db.Banners.AddRange(
            MakeBanner(id: 1, displayOrder: 1, isActive: false),
            MakeBanner(id: 2, displayOrder: 2, isActive: false)
        );
        await db.SaveChangesAsync();

        var repo   = new BannerRepository(db);
        var result = await repo.GetActiveBannersAsync();

        Assert.Empty(result);
    }

    // ── GetAllBannersAsync returns every record regardless of IsActive ─────────

    [Fact]
    public async Task GetAllBannersAsync_ReturnsAllBannersIncludingInactive()
    {
        await using var db = BuildDb(nameof(GetAllBannersAsync_ReturnsAllBannersIncludingInactive));

        db.Banners.AddRange(
            MakeBanner(id: 1, displayOrder: 2, isActive: true),
            MakeBanner(id: 2, displayOrder: 1, isActive: false)
        );
        await db.SaveChangesAsync();

        var repo   = new BannerRepository(db);
        var result = await repo.GetAllBannersAsync();

        Assert.Equal(2, result.Count);
    }
}
