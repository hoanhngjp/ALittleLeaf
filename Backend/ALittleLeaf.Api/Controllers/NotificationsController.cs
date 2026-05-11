using ALittleLeaf.Api.Services.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ALittleLeaf.Api.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _service;

        public NotificationsController(INotificationService service)
        {
            _service = service;
        }

        private long UserId => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>Get last 10 notifications for the current user.</summary>
        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            var items = await _service.GetUserNotificationsAsync(UserId);
            return Ok(items);
        }

        /// <summary>Get unread notification count.</summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var count = await _service.GetUnreadCountAsync(UserId);
            return Ok(new { count });
        }

        /// <summary>Mark a single notification as read.</summary>
        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            try
            {
                await _service.MarkAsReadAsync(UserId, id);
                return NoContent();
            }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (UnauthorizedAccessException) { return Forbid(); }
        }

        /// <summary>Mark all notifications as read.</summary>
        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _service.MarkAllAsReadAsync(UserId);
            return NoContent();
        }
    }
}
