using finder_work.Data;
using finder_work.Models;
using Microsoft.EntityFrameworkCore;

namespace finder_work.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string title, string message, string type = "Info", string? actionUrl = null, string? actionText = null);
        Task<List<Notification>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10);
        Task<int> GetUnreadCountAsync(int userId);
        Task MarkAsReadAsync(int notificationId, int userId);
        Task MarkAllAsReadAsync(int userId);
        Task DeleteNotificationAsync(int notificationId, int userId);
        Task CreateProfileApprovalNotificationAsync(int userId, string profileOwnerName, bool isApproved, string? rejectionReason = null);
        Task CreateSystemNotificationAsync(string title, string message, string type = "Info");
    }

    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateNotificationAsync(int userId, string title, string message, string type = "Info", string? actionUrl = null, string? actionText = null)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                ActionUrl = actionUrl,
                ActionText = actionText,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId, int page = 1, int pageSize = 10)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task MarkAsReadAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadDate = DateTime.UtcNow;
            }

            if (unreadNotifications.Any())
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteNotificationAsync(int notificationId, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateProfileApprovalNotificationAsync(int userId, string profileOwnerName, bool isApproved, string? rejectionReason = null)
        {
            string title, message, type, actionUrl, actionText;

            if (isApproved)
            {
                title = "Hồ sơ đã được duyệt";
                message = $"Chúc mừng! Hồ sơ của bạn đã được admin duyệt và hiển thị công khai.";
                type = "Success";
                actionUrl = "/Profile";
                actionText = "Xem hồ sơ";
            }
            else
            {
                title = "Hồ sơ bị từ chối";
                message = $"Hồ sơ của bạn đã bị từ chối. Lý do: {rejectionReason ?? "Không có lý do cụ thể"}";
                type = "Error";
                actionUrl = "/Profile";
                actionText = "Chỉnh sửa hồ sơ";
            }

            await CreateNotificationAsync(userId, title, message, type, actionUrl, actionText);
        }

        public async Task CreateSystemNotificationAsync(string title, string message, string type = "Info")
        {
            // Get all users to send system notification
            var userIds = await _context.Users
                .Where(u => u.Role == "User")
                .Select(u => u.UserId)
                .ToListAsync();

            var notifications = userIds.Select(userId => new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                CreatedDate = DateTime.UtcNow
            }).ToList();

            _context.Notifications.AddRange(notifications);
            await _context.SaveChangesAsync();
        }
    }
}