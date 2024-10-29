using ALittleLeaf.Models;

namespace ALittleLeaf.ViewModels
{
    public class HeaderCategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<HeaderCategoryViewModel> SubCategories { get; set; } = new List<HeaderCategoryViewModel>();
    }
}
