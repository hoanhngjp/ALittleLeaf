using System;

namespace ALittleLeaf.Api.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public long UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual User UserNavigation { get; set; } = null!;
        public virtual Product ProductNavigation { get; set; } = null!;
    }
}
