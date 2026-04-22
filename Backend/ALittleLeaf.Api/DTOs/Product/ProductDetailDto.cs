namespace ALittleLeaf.Api.DTOs.Product
{
    /// <summary>Full product detail including all images and category name.</summary>
    public class ProductDetailDto
    {
        public int      ProductId          { get; set; }
        public int      IdCategory         { get; set; }
        public string   CategoryName       { get; set; } = string.Empty;
        public string   ProductName        { get; set; } = string.Empty;
        public int      ProductPrice       { get; set; }
        public string?  ProductDescription { get; set; }
        public int      QuantityInStock    { get; set; }
        public bool     IsOnSale           { get; set; }
        public string?  PrimaryImage       { get; set; }
        public string[] Images             { get; set; } = [];
    }
}
