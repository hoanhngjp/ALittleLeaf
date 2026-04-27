using System;
using System.Collections.Generic;

namespace ALittleLeaf.Api.Models
{
    public partial class Bill
    {
        public int BillId { get; set; }

        public long IdUser { get; set; }

        public int IdAdrs { get; set; }

        public DateOnly DateCreated { get; set; }

        public int TotalAmount { get; set; }

        public int ShippingFee { get; set; }

        public string PaymentMethod { get; set; } = null!;

        public string PaymentStatus { get; set; } = null!;

        public bool IsConfirmed { get; set; }

        public string ShippingStatus { get; set; } = null!;

        public string? GhnOrderCode { get; set; }

        // Internal lifecycle: PENDING → CONFIRMED → SHIPPING → COMPLETED / CANCELLED
        public string OrderStatus { get; set; } = "PENDING";

        // Last human-readable tracking update from GHN webhook (message_display field)
        public string? TrackingMessage { get; set; }

        public string? Note { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

        public virtual AddressList IdAdrsNavigation { get; set; } = null!;

        public virtual User IdUserNavigation { get; set; } = null!;
    }

}