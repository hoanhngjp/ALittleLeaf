namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>Generic paginated result wrapper used across all admin list endpoints.</summary>
    public class PaginatedAdminResultDto<T>
    {
        public int            TotalItems { get; set; }
        public int            Page       { get; set; }
        public int            PageSize   { get; set; }
        public int            TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;
        public IEnumerable<T> Items      { get; set; } = [];
    }
}
