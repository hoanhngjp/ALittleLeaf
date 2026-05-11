using ALittleLeaf.Api.DTOs.Notification;
using ALittleLeaf.Api.Hubs;
using ALittleLeaf.Api.Repositories.Notification;
using Microsoft.AspNetCore.SignalR;

namespace ALittleLeaf.Api.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hub;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hub)
        {
            _repo = repo;
            _hub  = hub;
        }

        public async Task<List<NotificationDto>> GetUserNotificationsAsync(long userId)
        {
            var items = await _repo.GetByUserIdAsync(userId, 10);
            return items.Select(ToDto).ToList();
        }

        public Task<int> GetUnreadCountAsync(long userId)
            => _repo.GetUnreadCountAsync(userId);

        public async Task MarkAsReadAsync(long userId, int notificationId)
        {
            var n = await _repo.GetByIdAsync(notificationId)
                ?? throw new KeyNotFoundException("Notification not found.");
            if (n.UserId != userId)
                throw new UnauthorizedAccessException("Cannot mark another user's notification.");
            await _repo.MarkAsReadAsync(notificationId);
        }

        public Task MarkAllAsReadAsync(long userId)
            => _repo.MarkAllAsReadAsync(userId);

        public async Task SendOrderNotificationAsync(long userId, int orderId, string orderStatus)
        {
            var (title, message) = MapStatusToMessage(orderId, orderStatus);

            var notification = new Models.Notification
            {
                UserId         = userId,
                Title          = title,
                Message        = message,
                Type           = $"ORDER_{orderStatus}",
                RelatedOrderId = orderId,
                IsRead         = false,
                CreatedAt      = DateTime.UtcNow,
            };

            _repo.Add(notification);
            await _repo.SaveChangesAsync();

            var dto = ToDto(notification);
            await _hub.Clients.Group($"user_{userId}").SendAsync("ReceiveNotification", dto);
        }

        private static (string Title, string Message) MapStatusToMessage(int orderId, string status)
            => status switch
            {
                "CONFIRMED"  => ("Đơn hàng đã xác nhận",   $"Đơn hàng #{orderId} của bạn đã được xác nhận và đang được chuẩn bị."),
                "SHIPPING"   => ("Đơn hàng đang giao",      $"Đơn hàng #{orderId} đang trên đường giao đến bạn."),
                "COMPLETED"  => ("Đơn hàng hoàn thành",     $"Đơn hàng #{orderId} đã được giao thành công. Cảm ơn bạn đã mua hàng!"),
                "CANCELLED"  => ("Đơn hàng đã hủy",         $"Đơn hàng #{orderId} đã bị hủy."),
                _            => ("Cập nhật đơn hàng",       $"Đơn hàng #{orderId} đã được cập nhật trạng thái: {status}."),
            };

        private static NotificationDto ToDto(Models.Notification n) => new()
        {
            NotificationId = n.NotificationId,
            Title          = n.Title,
            Message        = n.Message,
            Type           = n.Type,
            RelatedOrderId = n.RelatedOrderId,
            IsRead         = n.IsRead,
            CreatedAt      = n.CreatedAt,
        };
    }
}
