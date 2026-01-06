using finder_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace finder_work.Controllers
{
    [Authorize(Roles = "User")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng.";
                    return RedirectToAction("Login", "Account");
                }

                const int pageSize = 10;
                var notifications = await _notificationService.GetUserNotificationsAsync(userId, page, pageSize);
                var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

                ViewBag.CurrentPage = page;
                ViewBag.UnreadCount = unreadCount;
                ViewBag.HasNotifications = notifications.Any();

                return View(notifications);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải thông báo.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetUnreadCount()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Json(new { success = false, error = "User not found" });
                }

                var count = await _notificationService.GetUnreadCountAsync(userId);
                return Json(new { success = true, count = count });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetRecentNotifications(int count = 5)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Json(new { success = false, error = "User not found" });
                }

                var notifications = await _notificationService.GetUserNotificationsAsync(userId, 1, count);
                
                var result = notifications.Select(n => new
                {
                    id = n.NotificationId,
                    title = n.Title,
                    message = n.Message,
                    type = n.Type,
                    isRead = n.IsRead,
                    createdDate = n.CreatedDate.ToString("dd/MM/yyyy HH:mm"),
                    actionUrl = n.ActionUrl,
                    actionText = n.ActionText,
                    timeAgo = GetTimeAgo(n.CreatedDate)
                });

                return Json(new { success = true, notifications = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> MarkAsRead(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Json(new { success = false, error = "User not found" });
                }

                await _notificationService.MarkAsReadAsync(id, userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> MarkAllAsRead()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Json(new { success = false, error = "User not found" });
                }

                await _notificationService.MarkAllAsReadAsync(userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    return Json(new { success = false, error = "User not found" });
                }

                await _notificationService.DeleteNotificationAsync(id, userId);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        private string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.UtcNow - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            if (timeSpan.TotalDays < 7)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            if (timeSpan.TotalDays < 30)
                return $"{(int)(timeSpan.TotalDays / 7)} tuần trước";
            if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            
            return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }
}