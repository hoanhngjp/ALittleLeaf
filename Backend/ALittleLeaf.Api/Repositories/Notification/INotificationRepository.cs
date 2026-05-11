using ALittleLeaf.Api.Models;

namespace ALittleLeaf.Api.Repositories.Notification
{
    public interface INotificationRepository
    {
        Task<List<Models.Notification>> GetByUserIdAsync(long userId, int take = 10);
        Task<int> GetUnreadCountAsync(long userId);
        Task<Models.Notification?> GetByIdAsync(int notificationId);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(long userId);
        void Add(Models.Notification notification);
        Task SaveChangesAsync();
    }
}
