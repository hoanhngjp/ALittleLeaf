namespace ALittleLeaf.Api.DTOs.Product
{
    /// <summary>Lightweight product summary used in list / search responses.</summary>
    public class ProductDto
    {
        public int     ProductId       { get; set; }
        public int     IdCategory      { get; set; }
        public string  ProductName     { get; set; } = string.Empty;
        public int     ProductPrice    { get; set; }
        public int     QuantityInStock { get; set; }
        public string? PrimaryImage    { get; set; }
    }
}
