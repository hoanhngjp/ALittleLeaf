namespace ALittleLeaf.Models
{
    public class SiteNavViewModel
    {
        public List<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();
        public decimal TotalPrice { get; set; }
        public SearchViewModel Search { get; set; } = new SearchViewModel();
    }

}
