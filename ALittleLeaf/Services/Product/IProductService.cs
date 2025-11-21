using ALittleLeaf.ViewModels;

namespace ALittleLeaf.Services.Product
{
    public interface IProductService
    {
        Task<ProductDetailViewModel> GetProductDetailAsync(int productId);
        Task<ProductServiceResult> GetPaginatedProductsAsync(int? categoryId, string keyword, int? minPrice, int? maxPrice, int page, int pageSize);
        Task<List<ProductViewModel>> GetLiveSearchResultsAsync(string keyword);
    }
}
