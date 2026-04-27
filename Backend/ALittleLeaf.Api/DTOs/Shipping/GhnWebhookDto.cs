using System.Text.Json.Serialization;

namespace ALittleLeaf.Api.DTOs.Shipping
{
    // GHN sends this payload to POST /api/shipping/webhook when an order status changes.
    public class GhnWebhookDto
    {
        [JsonPropertyName("OrderCode")]       public string  OrderCode      { get; set; } = string.Empty;
        [JsonPropertyName("Status")]          public string  Status         { get; set; } = string.Empty;
        [JsonPropertyName("CODAmount")]       public int     CodAmount      { get; set; }
        [JsonPropertyName("PaymentTypeID")]   public int     PaymentTypeId  { get; set; }
        [JsonPropertyName("message_display")] public string? MessageDisplay { get; set; }
    }
}
