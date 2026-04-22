namespace ALittleLeaf.Api.DTOs.Product
{
    /// <summary>Flat representation of a category (with optional sub-categories).</summary>
    public class CategoryDto
    {
        public int     CategoryId       { get; set; }
        public string  CategoryName     { get; set; } = string.Empty;
        public int?    CategoryParentId { get; set; }
        public string? CategoryImg      { get; set; }
        public IEnumerable<CategoryDto> SubCategories { get; set; } = [];
    }
}
