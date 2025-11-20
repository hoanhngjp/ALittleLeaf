using System;
using System.Collections.Generic;

namespace ALittleLeaf.Models
{
    public partial class Bill
    {
        public int BillId { get; set; }

        public long IdUser { get; set; }

        public int IdAdrs { get; set; }

        public DateOnly DateCreated { get; set; }

        public int TotalAmount { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public bool IsConfirmed { get; set; }

        public string ShippingStatus { get; set; } = null!;

        public string? Note { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

        public virtual AddressList IdAdrsNavigation { get; set; } = null!;

        public virtual User IdUserNavigation { get; set; } = null!;
    }

}