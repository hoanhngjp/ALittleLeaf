namespace ALittleLeaf.ViewModels
{
    public class OrderDetailViewModel
    {
        public int BillDetailId { get; set; }
        public int IdBill { get; set; }
        public int IdProduct { get; set; }
        public int IdCategory { get; set; }
        public string ProductName { get; set; }
        public string ProductImg { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int TotalPrice { get; set; }
    }
}
