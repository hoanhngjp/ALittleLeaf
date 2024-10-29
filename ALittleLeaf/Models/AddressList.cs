using System;
using System.Collections.Generic;

namespace ALittleLeaf.Models;

public partial class AddressList
{
    public int AdrsId { get; set; }

    public long IdUser { get; set; }

    public string AdrsFullname { get; set; } = null!;

    public string AdrsAddress { get; set; } = null!;

    public string AdrsPhone { get; set; } = null!;

    public bool AdrsIsDefault { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual User IdUserNavigation { get; set; } = null!;
}
