using System;
using System.Collections.Generic;

namespace ALittleLeaf.Models;

public partial class BillDetail
{
    public int BillDetailId { get; set; }

    public int IdBill { get; set; }

    public int IdProduct { get; set; }

    public int Quantity { get; set; }

    public int UnitPrice { get; set; }

    public int TotalPrice { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Bill IdBillNavigation { get; set; } = null!;

    public virtual Product IdProductNavigation { get; set; } = null!;
}
