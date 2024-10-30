namespace ALittleLeaf.ViewModels
{
    public class CollectionsMenuViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public List<CollectionsMenuViewModel> SubCategories { get; set; } = new List<CollectionsMenuViewModel>();
    }
}
