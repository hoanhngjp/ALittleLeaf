namespace ALittleLeaf.Api.Models
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Type { get; set; } = null!;
        public int? RelatedOrderId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public virtual User UserNavigation { get; set; } = null!;
    }
}
