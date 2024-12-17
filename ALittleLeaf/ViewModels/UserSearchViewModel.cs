using ALittleLeaf.Models;

namespace ALittleLeaf.ViewModels
{
    public class UserSearchViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public Paginate Pagination { get; set; }
    }
}
