using System.Net;
using System.Text;
using System.Text.Json;
using ALittleLeaf.Api.DTOs.Shipping;
using ALittleLeaf.Api.Options;
using ALittleLeaf.Api.Services.Shipping;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;

namespace ALittleLeaf.Tests;

public class GhnServiceTests
{
    // ── Helpers ───────────────────────────────────────────────────────────────

    private static GhnService BuildService(MockHttpMessageHandler mock, IMemoryCache? cache = null)
    {
        var client = mock.ToHttpClient();
        client.BaseAddress = new Uri("https://dev-online-gateway.ghn.vn/shiip/public-api/");

        var opts = Options.Create(new GhnOptions { ApiKey = "test-key", ShopId = 99 });
        cache ??= new MemoryCache(new MemoryCacheOptions());
        return new GhnService(client, cache, opts);
    }

    private static StringContent Json(object payload) =>
        new(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

    // ── Province deserialization ──────────────────────────────────────────────

    [Fact]
    public async Task GetProvincesAsync_DeserializesValidProvinces()
    {
        var mock = new MockHttpMessageHandler();
        mock.When("*/master-data/province")
            .Respond(HttpStatusCode.OK, Json(new
            {
                code = 200,
                message = "Success",
                data = new[]
                {
                    new { ProvinceID = 201, ProvinceName = "Hà Nội" },
                    new { ProvinceID = 202, ProvinceName = "Hồ Chí Minh" },
                }
            }));

        var svc = BuildService(mock);
        var result = await svc.GetProvincesAsync();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, p => p.ProvinceId == 201 && p.ProvinceName == "Hà Nội");
        Assert.Contains(result, p => p.ProvinceId == 202 && p.ProvinceName == "Hồ Chí Minh");
    }

    [Fact]
    public async Task GetProvincesAsync_FiltersOutSandboxJunkData()
    {
        var mock = new MockHttpMessageHandler();
        mock.When("*/master-data/province")
            .Respond(HttpStatusCode.OK, Json(new
            {
                code = 200,
                message = "Success",
                data = new[]
                {
                    new { ProvinceID = 201, ProvinceName = "Hà Nội" },
                    new { ProvinceID = 999, ProvinceName = "Test Province" },
                    new { ProvinceID = 998, ProvinceName = "Alert XSS" },
                    new { ProvinceID = 202, ProvinceName = "Hồ Chí Minh" },
                }
            }));

        var svc = BuildService(mock);
        var result = await svc.GetProvincesAsync();

        Assert.Equal(2, result.Count);
        Assert.DoesNotContain(result, p => p.ProvinceName.Contains("Test", StringComparison.OrdinalIgnoreCase));
        Assert.DoesNotContain(result, p => p.ProvinceName.Contains("Alert", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetProvincesAsync_ReturnsCachedResultOnSecondCall()
    {
        var mock = new MockHttpMessageHandler();
        // Expect exactly one HTTP hit — second call must be served from cache.
        // MockHttp throws MockException if an unmatched request is made after Expect is exhausted.
        mock.Expect("*/master-data/province")
            .Respond(HttpStatusCode.OK, Json(new
            {
                code = 200,
                message = "Success",
                data = new[] { new { ProvinceID = 201, ProvinceName = "Hà Nội" } }
            }));

        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc   = BuildService(mock, cache);

        var first  = await svc.GetProvincesAsync();
        var second = await svc.GetProvincesAsync(); // must not trigger another HTTP call

        Assert.Single(first);
        Assert.Single(second);
        mock.VerifyNoOutstandingExpectation(); // confirms exactly 1 HTTP request was made
    }

    [Fact]
    public async Task GetProvincesAsync_ReturnsEmptyList_WhenGhnReturnsNullData()
    {
        var mock = new MockHttpMessageHandler();
        mock.When("*/master-data/province")
            .Respond(HttpStatusCode.OK, Json(new { code = 200, message = "Success", data = (object?)null }));

        var svc    = BuildService(mock);
        var result = await svc.GetProvincesAsync();

        Assert.Empty(result);
    }

    // ── District deserialization ──────────────────────────────────────────────

    [Fact]
    public async Task GetDistrictsAsync_DeserializesDistrictsForProvince()
    {
        var mock = new MockHttpMessageHandler();
        mock.When("*/master-data/district*")
            .Respond(HttpStatusCode.OK, Json(new
            {
                code = 200,
                message = "Success",
                data = new[]
                {
                    new { DistrictID = 1442, DistrictName = "Quận 1", ProvinceID = 202 },
                    new { DistrictID = 1443, DistrictName = "Quận 2", ProvinceID = 202 },
                }
            }));

        var svc    = BuildService(mock);
        var result = await svc.GetDistrictsAsync(202);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, d => d.DistrictId == 1442 && d.DistrictName == "Quận 1");
    }

    [Fact]
    public async Task GetDistrictsAsync_CachesResultByProvinceId()
    {
        var mock = new MockHttpMessageHandler();
        // Expect exactly one HTTP hit for this province
        mock.Expect("*/master-data/district*")
            .Respond(HttpStatusCode.OK, Json(new
            {
                code = 200,
                message = "Success",
                data = new[] { new { DistrictID = 1442, DistrictName = "Quận 1", ProvinceID = 202 } }
            }));

        var cache = new MemoryCache(new MemoryCacheOptions());
        var svc   = BuildService(mock, cache);

        await svc.GetDistrictsAsync(202);
        await svc.GetDistrictsAsync(202); // second call — should use cache

        mock.VerifyNoOutstandingExpectation(); // confirms exactly 1 HTTP request was made
    }

    // ── Ward deserialization ──────────────────────────────────────────────────

    [Fact]
    public async Task GetWardsAsync_DeserializesWardsForDistrict()
    {
        var mock = new MockHttpMessageHandler();
        mock.When("*/master-data/ward*")
            .Respond(HttpStatusCode.OK, Json(new
            {
                code = 200,
                message = "Success",
                data = new[]
                {
                    new { WardCode = "20308", WardName = "Phường Bến Nghé", DistrictID = 1442 },
                    new { WardCode = "20310", WardName = "Phường Bến Thành", DistrictID = 1442 },
                }
            }));

        var svc    = BuildService(mock);
        var result = await svc.GetWardsAsync(1442);

        Assert.Equal(2, result.Count);
        Assert.Contains(result, w => w.WardCode == "20308" && w.WardName == "Phường Bến Nghé");
    }
}
