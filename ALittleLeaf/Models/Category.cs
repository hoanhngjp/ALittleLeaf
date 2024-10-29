using System;
using System.Collections.Generic;

namespace ALittleLeaf.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public int? CategoryParentId { get; set; }

    public string? CategoryImg { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    public virtual ICollection<Category> SubCategories { get; set; } = new List<Category>();

}
