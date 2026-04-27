using ALittleLeaf.Api.DTOs.Shipping;
using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Services.Shipping
{
    public interface IGhnService
    {
        Task<List<ProvinceDto>> GetProvincesAsync();
        Task<List<DistrictDto>> GetDistrictsAsync(int provinceId);
        Task<List<WardDto>>     GetWardsAsync(int districtId);
        Task<int>               CalculateShippingFeeAsync(int toDistrictId, string toWardCode, int weight, int insuranceValue);
        Task<string>            CreateShippingOrderAsync(Bill bill);
    }
}
