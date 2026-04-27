namespace ALittleLeaf.Api.DTOs.Shipping
{
    /// <summary>Generic envelope returned by every GHN API endpoint.</summary>
    public class GhnApiResponse<T>
    {
        public int    Code    { get; set; }
        public string Message { get; set; } = string.Empty;
        public T?     Data    { get; set; }
    }
}
