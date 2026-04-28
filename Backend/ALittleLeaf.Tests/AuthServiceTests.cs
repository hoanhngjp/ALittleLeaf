using ALittleLeaf.Api.Data;
using ALittleLeaf.Api.DTOs.Auth;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Services.Auth;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;

namespace ALittleLeaf.Tests;

public class AuthServiceTests
{
    // ── Fixture helpers ────────────────────────────────────────────────────────

    /// <summary>
    /// Builds an isolated in-memory DbContext (unique name per call) so tests
    /// don't share state even when run in parallel.
    /// </summary>
    private static AlittleLeafDecorContext BuildDb(string dbName)
    {
        var opts = new DbContextOptionsBuilder<AlittleLeafDecorContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new AlittleLeafDecorContext(opts);
    }

    /// <summary>
    /// Minimal IConfiguration that satisfies JWT + Google:ClientId requirements.
    /// </summary>
    private static IConfiguration BuildConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"]                     = "TestSecret_MustBe32CharsOrLonger!!",
                ["Jwt:Issuer"]                     = "TestIssuer",
                ["Jwt:Audience"]                   = "TestAudience",
                ["Jwt:AccessTokenExpirationMinutes"] = "60",
                ["Jwt:RefreshTokenExpirationDays"]  = "7",
                ["Google:ClientId"]                = "test-google-client-id",
            })
            .Build();

    private static (AuthService svc,
                    AlittleLeafDecorContext db,
                    Mock<IGoogleTokenValidator> validatorMock)
        BuildSut(string dbName)
    {
        var db            = BuildDb(dbName);
        var config        = BuildConfig();
        var validatorMock = new Mock<IGoogleTokenValidator>();
        var svc           = new AuthService(db, config, validatorMock.Object);
        return (svc, db, validatorMock);
    }

    private static GoogleJsonWebSignature.Payload MakePayload(
        string email      = "user@gmail.com",
        bool emailVerified = true,
        string subject    = "google-sub-123",
        string name       = "Test User") =>
        new()
        {
            Email         = email,
            EmailVerified = emailVerified,
            Subject       = subject,
            Name          = name,
        };

    // ── Scenario 1: Unverified email is rejected ───────────────────────────────

    [Fact]
    public async Task GoogleLoginAsync_UnverifiedEmail_ReturnsFailure()
    {
        var (svc, _, validatorMock) = BuildSut(nameof(GoogleLoginAsync_UnverifiedEmail_ReturnsFailure));

        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(MakePayload(emailVerified: false));

        var result = await svc.GoogleLoginAsync("fake-id-token");

        Assert.False(result.Succeeded);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("chưa được xác minh", result.ErrorMessage);
    }

    // ── Scenario 2: Brand-new Google user is created ──────────────────────────

    [Fact]
    public async Task GoogleLoginAsync_NewUser_CreatesAccountAndReturnsTokens()
    {
        var (svc, db, validatorMock) =
            BuildSut(nameof(GoogleLoginAsync_NewUser_CreatesAccountAndReturnsTokens));

        const string email    = "newuser@gmail.com";
        const string googleId = "google-sub-new-999";

        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(MakePayload(email: email, subject: googleId));

        var result = await svc.GoogleLoginAsync("any-token");

        Assert.True(result.Succeeded);
        Assert.NotEmpty(result.AccessToken!);
        Assert.NotEmpty(result.RefreshToken!);
        Assert.Equal(email, result.User!.UserEmail);

        // Verify the user row was persisted
        var saved = await db.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
        Assert.NotNull(saved);
        Assert.Equal("google",   saved.AuthProvider);
        Assert.Equal(googleId,   saved.GoogleId);
        Assert.Null(saved.UserPassword);   // Google accounts have no local password
        Assert.Equal("customer", saved.UserRole);
        Assert.True(saved.UserIsActive);
    }

    // ── Scenario 3: Existing local user is merged — password kept intact ───────

    [Fact]
    public async Task GoogleLoginAsync_ExistingLocalUser_MergesAndKeepsPasswordIntact()
    {
        var (svc, db, validatorMock) =
            BuildSut(nameof(GoogleLoginAsync_ExistingLocalUser_MergesAndKeepsPasswordIntact));

        const string email          = "local@example.com";
        const string originalHash   = "AQAAAAIAAYag_hashed_password_here==";
        const string googleId       = "google-sub-merge-42";

        // Seed an existing local account (no AuthProvider set)
        db.Users.Add(new User
        {
            UserEmail    = email,
            UserPassword = originalHash,
            UserFullname = "Local User",
            UserSex      = false,
            UserBirthday = new DateOnly(1995, 1, 1),
            UserIsActive = true,
            UserRole     = "customer",
            CreatedAt    = DateTime.UtcNow,
            UpdatedAt    = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();

        validatorMock
            .Setup(v => v.ValidateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(MakePayload(email: email, subject: googleId));

        var result = await svc.GoogleLoginAsync("any-token");

        Assert.True(result.Succeeded);
        Assert.Equal(email, result.User!.UserEmail);

        // Account is merged: GoogleId set, AuthProvider set, password NOT wiped
        var merged = await db.Users.FirstOrDefaultAsync(u => u.UserEmail == email);
        Assert.NotNull(merged);
        Assert.Equal(googleId,     merged.GoogleId);
        Assert.Equal("google",     merged.AuthProvider);
        Assert.Equal(originalHash, merged.UserPassword);   // password must be preserved
    }
}
