using finder_work.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finder_work.ViewComponents
{
    public class NotificationMenuViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;

        public NotificationMenuViewComponent(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            try
            {
                // Check if user is authenticated
                if (HttpContext.User?.Identity?.IsAuthenticated != true)
                {
                    return View("Empty");
                }

                var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                {
                    return View("Empty");
                }

                // Get recent notifications and unread count
                var recentNotifications = await _notificationService.GetUserNotificationsAsync(userId, 1, 5);
                var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

                ViewBag.UnreadCount = unreadCount;
                ViewBag.HasNotifications = recentNotifications.Any();

                return View(recentNotifications);
            }
            catch (Exception)
            {
                // Return empty view on error
                return View("Empty");
            }
        }
    }
}