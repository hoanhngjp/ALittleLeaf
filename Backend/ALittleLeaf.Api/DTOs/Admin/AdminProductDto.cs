namespace ALittleLeaf.Api.DTOs.Admin
{
    /// <summary>Full product record returned to the admin panel.</summary>
    public class AdminProductDto
    {
        public int     ProductId          { get; set; }
        public int     IdCategory         { get; set; }
        public string  CategoryName       { get; set; } = string.Empty;
        public string  ProductName        { get; set; } = string.Empty;
        public int     ProductPrice       { get; set; }
        public string? ProductDescription { get; set; }
        public int     QuantityInStock    { get; set; }
        public bool    IsOnSale           { get; set; }
        public string? PrimaryImage       { get; set; }
        public IEnumerable<AdminProductImageDto> Images { get; set; } = [];
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AdminProductImageDto
    {
        public int    ImgId     { get; set; }
        public string ImgName   { get; set; } = string.Empty;
        public bool   IsPrimary { get; set; }
    }
}
