using ALittleLeaf.Api.DTOs.Notification;

namespace ALittleLeaf.Api.Services.Notification
{
    public interface INotificationService
    {
        Task<List<NotificationDto>> GetUserNotificationsAsync(long userId);
        Task<int> GetUnreadCountAsync(long userId);
        Task MarkAsReadAsync(long userId, int notificationId);
        Task MarkAllAsReadAsync(long userId);
        Task SendOrderNotificationAsync(long userId, int orderId, string orderStatus);
    }
}
