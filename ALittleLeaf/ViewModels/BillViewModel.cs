namespace ALittleLeaf.ViewModels
{
    public class BillViewModel 
    {
        public int BillId { get; set; }
        public DateOnly DateCreated { get; set; }
        public int TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public string PaymentStatus { get; set; } = null!;
        public bool IsConfirmed { get; set; }
        public string ShippingStatus { get; set; } = null!;
    }
}
