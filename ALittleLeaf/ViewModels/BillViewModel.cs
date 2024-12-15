namespace ALittleLeaf.ViewModels
{
    public class BillViewModel 
    {
        public int BillId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public DateOnly DateCreated { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public string? Note { get; set; }
        public string ShippingStatus { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }

    }
}
