using ALittleLeaf.Models;

namespace ALittleLeaf.ViewModels
{
    public class ProductServiceResult
    {
        public List<ProductViewModel> Products { get; set; } 
        public Paginate Pagination { get; set; } 
        public string PageTitle { get; set; }    
        public int? CategoryId { get; set; }     
    }
}
