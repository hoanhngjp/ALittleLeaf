namespace ALittleLeaf.Services.Cart
{
    public class CartUpdateResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int LineItemTotal { get; set; }
        public int TotalPrice { get; set; }
        public int TotalItems { get; set; }
    }
}
