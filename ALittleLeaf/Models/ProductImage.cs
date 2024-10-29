using System;
using System.Collections.Generic;

namespace ALittleLeaf.Models;

public partial class ProductImage
{
    public int ImgId { get; set; }

    public int IdProduct { get; set; }

    public string ImgName { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Product IdProductNavigation { get; set; } = null!;
}
