using finder_work.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace finder_work.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Dashboard statistics using Entity Framework directly
            var stats = new
            {
                TotalUsers = await _context.Users.CountAsync(u => u.Role == "User"),
                TotalAdmins = await _context.Users.CountAsync(u => u.Role == "Admin"),
                PendingProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Pending"),
                ApprovedProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Approved"),
                RejectedProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Rejected"),
                TotalProfiles = await _context.UserProfiles.CountAsync()
            };

            ViewBag.Stats = stats;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> PendingProfiles()
        {
            var pendingProfiles = await _context.UserProfiles
                .Include(p => p.User)
                .Where(p => p.ApprovalStatus == "Pending")
                .OrderByDescending(p => p.CreatedDate)
                .ToListAsync();

            return View(pendingProfiles);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveProfile(int profileId)
        {
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null)
            {
                return NotFound();
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            profile.ApprovalStatus = "Approved";
            profile.ApprovedBy = adminId;
            profile.ApprovedDate = DateTime.UtcNow;
            profile.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Hồ sơ đã được duyệt thành công!";
            return RedirectToAction(nameof(PendingProfiles));
        }

        [HttpPost]
        public async Task<IActionResult> RejectProfile(int profileId, string rejectionReason)
        {
            var profile = await _context.UserProfiles.FindAsync(profileId);
            if (profile == null)
            {
                return NotFound();
            }

            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            profile.ApprovalStatus = "Rejected";
            profile.ApprovedBy = adminId;
            profile.ApprovedDate = DateTime.UtcNow;
            profile.RejectionReason = rejectionReason;
            profile.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Hồ sơ đã bị từ chối!";
            return RedirectToAction(nameof(PendingProfiles));
        }
    }
}
