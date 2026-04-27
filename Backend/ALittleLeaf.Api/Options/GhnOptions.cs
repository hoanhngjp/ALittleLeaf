namespace ALittleLeaf.Api.Options
{
    public class GhnOptions
    {
        public const string Section = "Ghn";

        public string ApiKey  { get; set; } = string.Empty;
        public int    ShopId  { get; set; }
        public string BaseUrl { get; set; } = "https://dev-online-gateway.ghn.vn/shiip/public-api";
    }
}
