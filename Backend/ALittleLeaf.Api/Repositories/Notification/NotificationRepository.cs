using ALittleLeaf.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ALittleLeaf.Api.Repositories.Notification
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly AlittleLeafDecorContext _context;

        public NotificationRepository(AlittleLeafDecorContext context)
        {
            _context = context;
        }

        public async Task<List<Models.Notification>> GetByUserIdAsync(long userId, int take = 10)
            => await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();

        public async Task<int> GetUnreadCountAsync(long userId)
            => await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task<Models.Notification?> GetByIdAsync(int notificationId)
            => await _context.Notifications.FindAsync(notificationId);

        public async Task MarkAsReadAsync(int notificationId)
        {
            var n = await _context.Notifications.FindAsync(notificationId);
            if (n != null && !n.IsRead)
            {
                n.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(long userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();
            foreach (var n in unread) n.IsRead = true;
            await _context.SaveChangesAsync();
        }

        public void Add(Models.Notification notification)
            => _context.Notifications.Add(notification);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();
    }
}
