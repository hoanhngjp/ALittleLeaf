using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ALittleLeaf.Api.Models
{
    public partial class Product
    {
        public int ProductId { get; set; }

        public int IdCategory { get; set; }

        public string ProductName { get; set; } = null!;

        public int ProductPrice { get; set; }

        public string? ProductDescription { get; set; }

        public int QuantityInStock { get; set; }

        public bool IsOnSale { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

        public virtual Category? IdCategoryNavigation { get; set; }

        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    }
}


