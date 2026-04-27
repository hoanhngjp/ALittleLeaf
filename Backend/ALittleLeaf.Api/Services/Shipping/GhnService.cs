using System.Net.Http.Json;
using System.Text.Json;
using ALittleLeaf.Api.DTOs.Shipping;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace ALittleLeaf.Api.Services.Shipping
{
    public class GhnService : IGhnService
    {
        private readonly HttpClient    _http;
        private readonly IMemoryCache  _cache;
        private readonly int           _shopId;

        private static readonly TimeSpan CacheDuration = TimeSpan.FromHours(24);
        private const string ProvincesCacheKey = "ghn:provinces";

        // GHN returns PascalCase keys with non-standard casing (e.g. "ProvinceID", "WardCode").
        // Case-insensitive matching maps them to our clean C# property names without needing
        // [JsonPropertyName] attributes on the DTOs (which would break ASP.NET Core's camelCase
        // serialization to the frontend).
        private static readonly JsonSerializerOptions GhnJsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
        };

        // Sandbox junk-data keywords — test provinces created by other developers on the shared sandbox
        private static readonly string[] SandboxJunkKeywords = ["Test", "Alert"];

        public GhnService(HttpClient http, IMemoryCache cache, IOptions<GhnOptions> opts)
        {
            _http   = http;
            _cache  = cache;
            _shopId = opts.Value.ShopId;
        }

        public async Task<List<ProvinceDto>> GetProvincesAsync()
        {
            if (_cache.TryGetValue(ProvincesCacheKey, out List<ProvinceDto>? cached))
                return cached!;

            var res = await _http.GetFromJsonAsync<GhnApiResponse<List<ProvinceDto>>>(
                "master-data/province", GhnJsonOptions);

            var data = (res?.Data ?? [])
                .Where(p => !SandboxJunkKeywords.Any(kw =>
                    p.ProvinceName.Contains(kw, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            _cache.Set(ProvincesCacheKey, data, CacheDuration);
            return data;
        }

        public async Task<List<DistrictDto>> GetDistrictsAsync(int provinceId)
        {
            var key = $"ghn:districts:{provinceId}";
            if (_cache.TryGetValue(key, out List<DistrictDto>? cached))
                return cached!;

            var res = await _http.GetFromJsonAsync<GhnApiResponse<List<DistrictDto>>>(
                $"master-data/district?province_id={provinceId}", GhnJsonOptions);
            var data = res?.Data ?? [];

            _cache.Set(key, data, CacheDuration);
            return data;
        }

        public async Task<List<WardDto>> GetWardsAsync(int districtId)
        {
            var res = await _http.GetFromJsonAsync<GhnApiResponse<List<WardDto>>>(
                $"master-data/ward?district_id={districtId}", GhnJsonOptions);
            return res?.Data ?? [];
        }

        public async Task<int> CalculateShippingFeeAsync(int toDistrictId, string toWardCode, int weight, int insuranceValue)
        {
            var safeInsurance = Math.Min(insuranceValue, 5_000_000);

            var request = new HttpRequestMessage(HttpMethod.Post, "v2/shipping-order/fee")
            {
                Content = JsonContent.Create(new
                {
                    service_type_id = 2,
                    to_district_id  = toDistrictId,
                    to_ward_code    = toWardCode,
                    weight          = weight,
                    insurance_value = safeInsurance,
                    length          = 20,
                    width           = 20,
                    height          = 20,
                }),
            };
            request.Headers.Add("ShopId", _shopId.ToString());

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"GHN fee API returned {(int)response.StatusCode}: {body}");
            }

            var res = await response.Content.ReadFromJsonAsync<GhnApiResponse<ShippingFeeData>>(GhnJsonOptions);
            return res?.Data?.Total ?? 0;
        }

        public async Task<string> CreateShippingOrderAsync(Bill bill)
        {
            var adrs = bill.IdAdrsNavigation
                       ?? throw new InvalidOperationException("Bill has no shipping address loaded.");

            // Build item list from BillDetails; fall back to a single generic item when details are missing.
            var items = bill.BillDetails?.Select(d => new
            {
                name     = d.IdProductNavigation?.ProductName ?? $"Product {d.IdProduct}",
                quantity = d.Quantity,
                weight   = 200,
            }).ToArray<object>()
            ?? [new { name = "Đơn hàng ALittleLeaf", quantity = 1, weight = 500 }];

            int totalWeight = Math.Max(
                bill.BillDetails?.Sum(d => d.Quantity * 200) ?? 500,
                1);

            var payload = new
            {
                payment_type_id  = 2,        // seller pays shipping
                note             = bill.Note ?? string.Empty,
                required_note    = "CHOXEMHANGKHONGTHU",
                to_name          = adrs.AdrsFullname,
                to_phone         = adrs.AdrsPhone,
                to_address       = adrs.AdrsAddress,
                to_ward_code     = adrs.WardCode ?? string.Empty,
                to_district_id   = adrs.DistrictId ?? 0,
                weight           = totalWeight,
                length           = 20,
                width            = 20,
                height           = 20,
                insurance_value  = Math.Min(bill.TotalAmount, 5_000_000),
                service_type_id  = 2,
                items,
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "v2/shipping-order/create")
            {
                Content = JsonContent.Create(payload),
            };
            request.Headers.Add("ShopId", _shopId.ToString());

            var response = await _http.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException(
                    $"GHN create-order API returned {(int)response.StatusCode}: {body}");
            }

            var res = await response.Content
                .ReadFromJsonAsync<GhnApiResponse<GhnCreateOrderData>>(GhnJsonOptions);

            return res?.Data?.OrderCode
                   ?? throw new InvalidOperationException("GHN did not return an order code.");
        }
    }
}

// Internal DTO — GHN returns snake_case keys so [JsonPropertyName] is required here.
// PropertyNameCaseInsensitive handles case only; it does NOT map snake_case → PascalCase.
file sealed class GhnCreateOrderData
{
    [System.Text.Json.Serialization.JsonPropertyName("order_code")]
    public string OrderCode { get; set; } = string.Empty;
}
