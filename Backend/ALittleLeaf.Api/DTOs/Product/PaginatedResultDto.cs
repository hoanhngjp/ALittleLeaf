namespace ALittleLeaf.Api.DTOs.Product
{
    /// <summary>Generic paginated result wrapper returned by list endpoints.</summary>
    public class PaginatedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = [];
        public int TotalItems   { get; set; }
        public int Page         { get; set; }
        public int PageSize     { get; set; }
        public int TotalPages   => (int)Math.Ceiling((double)TotalItems / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext     => Page < TotalPages;
    }
}
