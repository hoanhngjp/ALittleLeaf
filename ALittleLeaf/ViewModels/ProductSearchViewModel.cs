using ALittleLeaf.Models;

namespace ALittleLeaf.ViewModels
{
    public class ProductSearchViewModel
    {
        public IEnumerable<ProductDetailViewModel> Products { get; set; }
        public Paginate Pagination { get; set; }
    }
}
