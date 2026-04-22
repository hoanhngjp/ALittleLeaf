namespace ALittleLeaf.Api.DTOs.Order
{
    public class OrderDetailDto : OrderDto
    {
        public IEnumerable<OrderLineItemDto> Items { get; set; } = [];
    }
}
