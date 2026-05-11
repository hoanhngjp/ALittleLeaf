using ALittleLeaf.Api.Hubs;
using ALittleLeaf.Api.Models;
using ALittleLeaf.Api.Repositories.Notification;
using ALittleLeaf.Api.Services.Notification;
using ALittleLeaf.Tests.Helpers;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace ALittleLeaf.Tests.Services
{
    public class NotificationServiceTests : IDisposable
    {
        private readonly ALittleLeaf.Api.Data.AlittleLeafDecorContext _context;
        private readonly NotificationService _service;
        private readonly Mock<IHubContext<NotificationHub>> _mockHub;
        private readonly Mock<IClientProxy> _mockClientProxy;

        private const long UserId  = 42;
        private const int  OrderId = 99;

        public NotificationServiceTests()
        {
            _context         = DbContextFactory.Create();
            _mockHub         = new Mock<IHubContext<NotificationHub>>();
            _mockClientProxy = new Mock<IClientProxy>();

            var mockClients = new Mock<IHubClients>();
            mockClients.Setup(c => c.Group(It.IsAny<string>())).Returns(_mockClientProxy.Object);
            _mockHub.Setup(h => h.Clients).Returns(mockClients.Object);

            var repo = new NotificationRepository(_context);
            _service = new NotificationService(repo, _mockHub.Object);

            SeedUser();
        }

        public void Dispose() => DbContextFactory.Destroy(_context);

        private void SeedUser()
        {
            if (!_context.Users.Any(u => u.UserId == UserId))
            {
                _context.Users.Add(new User
                {
                    UserId      = UserId,
                    UserEmail   = "notify@test.com",
                    UserFullname = "Test User",
                    UserSex     = false,
                    UserBirthday = DateOnly.FromDateTime(DateTime.UtcNow),
                    UserIsActive = true,
                    UserRole    = "user",
                    CreatedAt   = DateTime.UtcNow,
                    UpdatedAt   = DateTime.UtcNow,
                });
                _context.SaveChanges();
            }
        }

        [Fact]
        public async Task SendOrderNotification_Confirmed_PersistsAndPushesCorrectMessage()
        {
            await _service.SendOrderNotificationAsync(UserId, OrderId, "CONFIRMED");

            var saved = _context.Notifications.Single(n => n.UserId == UserId);
            Assert.Equal("Đơn hàng đã xác nhận", saved.Title);
            Assert.Equal("ORDER_CONFIRMED", saved.Type);
            Assert.Equal(OrderId, saved.RelatedOrderId);
            Assert.False(saved.IsRead);

            _mockClientProxy.Verify(
                c => c.SendCoreAsync("ReceiveNotification", It.IsAny<object[]>(), default),
                Times.Once);
        }

        [Fact]
        public async Task SendOrderNotification_Cancelled_PushesCorrectVietnameseMessage()
        {
            await _service.SendOrderNotificationAsync(UserId, OrderId, "CANCELLED");

            var saved = _context.Notifications.Single(n => n.UserId == UserId);
            Assert.Equal("Đơn hàng đã hủy", saved.Title);
            Assert.Equal("ORDER_CANCELLED", saved.Type);
        }

        [Fact]
        public async Task MarkAsRead_WrongUser_ThrowsUnauthorized()
        {
            await _service.SendOrderNotificationAsync(UserId, OrderId, "CONFIRMED");
            var n = _context.Notifications.Single();

            await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _service.MarkAsReadAsync(UserId + 1, n.NotificationId));
        }

        [Fact]
        public async Task GetUserNotifications_ReturnsLast10OrderedDescending()
        {
            for (int i = 1; i <= 12; i++)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId    = UserId,
                    Title     = $"N{i}",
                    Message   = "msg",
                    Type      = "ORDER_CONFIRMED",
                    IsRead    = false,
                    CreatedAt = DateTime.UtcNow.AddMinutes(i),
                });
            }
            await _context.SaveChangesAsync();

            var results = await _service.GetUserNotificationsAsync(UserId);

            Assert.Equal(10, results.Count);
            // Most recent first
            Assert.True(results[0].CreatedAt >= results[1].CreatedAt);
        }

        [Fact]
        public async Task MarkAllAsRead_SetsAllUnreadToRead()
        {
            for (int i = 0; i < 3; i++)
            {
                _context.Notifications.Add(new Notification
                {
                    UserId    = UserId,
                    Title     = $"T{i}",
                    Message   = "msg",
                    Type      = "ORDER_CONFIRMED",
                    IsRead    = false,
                    CreatedAt = DateTime.UtcNow,
                });
            }
            await _context.SaveChangesAsync();

            await _service.MarkAllAsReadAsync(UserId);

            var unread = _context.Notifications.Count(n => n.UserId == UserId && !n.IsRead);
            Assert.Equal(0, unread);
        }
    }
}
