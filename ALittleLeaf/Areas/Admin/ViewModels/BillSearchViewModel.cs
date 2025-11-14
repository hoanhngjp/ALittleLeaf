using ALittleLeaf.Models;

namespace ALittleLeaf.ViewModels
{
    public class BillSearchViewModel
    {
        public IEnumerable<BillViewModel> Bills { get; set; }
        public Paginate Pagination { get; set; }
    }
}
