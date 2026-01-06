using finder_work.Data;
using finder_work.Models;
using finder_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finder_work.Areas.Admin.Controllers
{
    [Area("Admin")]
    // [Authorize(Roles = "Admin")] // Tạm thời comment để test
    public class ManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public ManagementController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }



        // Categories Management
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .Include(c => c.Professions)
                .ToListAsync();
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(string categoryName, string categoryDescription)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(categoryName))
                {
                    var category = new Category
                    {
                        CategoryName = categoryName,
                        CategoryDescription = categoryDescription
                    };
                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Danh mục đã được tạo thành công!";
                }
                else
                {
                    TempData["Error"] = "Tên danh mục không được để trống!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tạo danh mục: {ex.Message}";
            }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(int categoryId, string categoryName, string categoryDescription)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null && !string.IsNullOrWhiteSpace(categoryName))
            {
                category.CategoryName = categoryName;
                category.CategoryDescription = categoryDescription;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Danh mục đã được cập nhật thành công!";
            }
            else
            {
                TempData["Error"] = "Không tìm thấy danh mục hoặc tên danh mục không hợp lệ!";
            }
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Professions)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            
            if (category != null)
            {
                if (category.Professions.Any())
                {
                    TempData["Error"] = "Không thể xóa danh mục này vì còn có nghề nghiệp liên quan!";
                }
                else
                {
                    _context.Categories.Remove(category);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Danh mục đã được xóa thành công!";
                }
            }
            else
            {
                TempData["Error"] = "Không tìm thấy danh mục!";
            }
            return RedirectToAction(nameof(Categories));
        }

        // Skill Types Management
        public async Task<IActionResult> SkillTypes()
        {
            var skillTypes = await _context.SkillTypes
                .Include(st => st.Skills)
                .ToListAsync();
            return View(skillTypes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSkillType(string skillTypeName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(skillTypeName))
                {
                    var skillType = new SkillType
                    {
                        SkillTypeName = skillTypeName
                    };
                    _context.SkillTypes.Add(skillType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại kỹ năng đã được tạo thành công!";
                }
                else
                {
                    TempData["Error"] = "Tên loại kỹ năng không được để trống!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tạo loại kỹ năng: {ex.Message}";
            }
            return RedirectToAction(nameof(SkillTypes));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSkillType(int skillTypeId, string skillTypeName)
        {
            try
            {
                var skillType = await _context.SkillTypes.FindAsync(skillTypeId);
                
                if (skillType != null && !string.IsNullOrWhiteSpace(skillTypeName))
                {
                    skillType.SkillTypeName = skillTypeName;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại kỹ năng đã được cập nhật thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại kỹ năng hoặc tên loại kỹ năng không hợp lệ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật loại kỹ năng: {ex.Message}";
            }
            return RedirectToAction(nameof(SkillTypes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSkillType(int skillTypeId)
        {
            try
            {
                var skillType = await _context.SkillTypes
                    .Include(st => st.Skills)
                    .FirstOrDefaultAsync(st => st.SkillTypeId == skillTypeId);
                
                if (skillType != null)
                {
                    if (skillType.Skills.Any())
                    {
                        TempData["Error"] = "Không thể xóa loại kỹ năng này vì còn có kỹ năng liên quan!";
                    }
                    else
                    {
                        _context.SkillTypes.Remove(skillType);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Loại kỹ năng đã được xóa thành công!";
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại kỹ năng!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa loại kỹ năng: {ex.Message}";
            }
            return RedirectToAction(nameof(SkillTypes));
        }


        // Certificate Types Management
        public async Task<IActionResult> CertificateTypes()
        {
            var certificateTypes = await _context.CertificateTypes
                .Include(ct => ct.Certificates)
                .ToListAsync();
            return View(certificateTypes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCertificateType(string certificateTypeName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(certificateTypeName))
                {
                    var certificateType = new CertificateType
                    {
                        CertificateTypeName = certificateTypeName
                    };
                    _context.CertificateTypes.Add(certificateType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại chứng chỉ đã được tạo thành công!";
                }
                else
                {
                    TempData["Error"] = "Tên loại chứng chỉ không được để trống!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tạo loại chứng chỉ: {ex.Message}";
            }
            return RedirectToAction(nameof(CertificateTypes));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCertificateType(int certificateTypeId, string certificateTypeName)
        {
            try
            {
                var certificateType = await _context.CertificateTypes.FindAsync(certificateTypeId);
                
                if (certificateType != null && !string.IsNullOrWhiteSpace(certificateTypeName))
                {
                    certificateType.CertificateTypeName = certificateTypeName;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại chứng chỉ đã được cập nhật thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại chứng chỉ hoặc tên loại chứng chỉ không hợp lệ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật loại chứng chỉ: {ex.Message}";
            }
            return RedirectToAction(nameof(CertificateTypes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCertificateType(int certificateTypeId)
        {
            try
            {
                var certificateType = await _context.CertificateTypes
                    .Include(ct => ct.Certificates)
                    .FirstOrDefaultAsync(ct => ct.CertificateTypeId == certificateTypeId);
                
                if (certificateType != null)
                {
                    if (certificateType.Certificates.Any())
                    {
                        TempData["Error"] = "Không thể xóa loại chứng chỉ này vì còn có chứng chỉ liên quan!";
                    }
                    else
                    {
                        _context.CertificateTypes.Remove(certificateType);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Loại chứng chỉ đã được xóa thành công!";
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại chứng chỉ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa loại chứng chỉ: {ex.Message}";
            }
            return RedirectToAction(nameof(CertificateTypes));
        }


        // Degree Types Management
        public async Task<IActionResult> DegreeTypes()
        {
            var degreeTypes = await _context.DegreeTypes
                .Include(dt => dt.Degrees)
                .ToListAsync();
            return View(degreeTypes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDegreeType(string degreeTypeName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(degreeTypeName))
                {
                    var degreeType = new DegreeType
                    {
                        DegreeTypeName = degreeTypeName
                    };
                    _context.DegreeTypes.Add(degreeType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại bằng cấp đã được tạo thành công!";
                }
                else
                {
                    TempData["Error"] = "Tên loại bằng cấp không được để trống!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tạo loại bằng cấp: {ex.Message}";
            }
            return RedirectToAction(nameof(DegreeTypes));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDegreeType(int degreeTypeId, string degreeTypeName)
        {
            try
            {
                var degreeType = await _context.DegreeTypes.FindAsync(degreeTypeId);
                
                if (degreeType != null && !string.IsNullOrWhiteSpace(degreeTypeName))
                {
                    degreeType.DegreeTypeName = degreeTypeName;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại bằng cấp đã được cập nhật thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại bằng cấp hoặc tên loại bằng cấp không hợp lệ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật loại bằng cấp: {ex.Message}";
            }
            return RedirectToAction(nameof(DegreeTypes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDegreeType(int degreeTypeId)
        {
            try
            {
                var degreeType = await _context.DegreeTypes
                    .Include(dt => dt.Degrees)
                    .FirstOrDefaultAsync(dt => dt.DegreeTypeId == degreeTypeId);
                
                if (degreeType != null)
                {
                    if (degreeType.Degrees.Any())
                    {
                        TempData["Error"] = "Không thể xóa loại bằng cấp này vì còn có bằng cấp liên quan!";
                    }
                    else
                    {
                        _context.DegreeTypes.Remove(degreeType);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Loại bằng cấp đã được xóa thành công!";
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại bằng cấp!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa loại bằng cấp: {ex.Message}";
            }
            return RedirectToAction(nameof(DegreeTypes));
        }

        // User Accounts Management
        public async Task<IActionResult> UserAccounts()
        {
            var users = await _context.Users
                .Where(u => u.Role == "User")
                .ToListAsync();
            return View(users);
        }

        // Job Profiles Management with Statistics
        public async Task<IActionResult> JobProfiles()
        {
            // Get statistics
            var totalProfiles = await _context.UserProfiles.CountAsync();
            var approvedProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Approved");
            var pendingProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Pending");
            var rejectedProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Rejected");

            // Get profiles by category
            var profilesByCategory = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved")
                .SelectMany(p => p.UserProfileProfessions)
                .GroupBy(upp => upp.Profession.Category.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            // Get profiles by location
            var profilesByLocation = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved" && !string.IsNullOrEmpty(p.Location))
                .GroupBy(p => p.Location)
                .Select(g => new { Location = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToListAsync();

            // Get recent profiles
            var recentProfiles = await _context.UserProfiles
                .Include(p => p.User)
                .Include(p => p.UserProfileProfessions)
                    .ThenInclude(upp => upp.Profession)
                        .ThenInclude(pr => pr.Category)
                .OrderByDescending(p => p.UpdatedDate)
                .Take(10)
                .ToListAsync();

            // Get salary statistics
            var salaryStats = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved" && p.ExpectedSalary.HasValue)
                .Select(p => p.ExpectedSalary.Value)
                .ToListAsync();

            var averageSalary = salaryStats.Any() ? salaryStats.Average() : 0;
            var minSalary = salaryStats.Any() ? salaryStats.Min() : 0;
            var maxSalary = salaryStats.Any() ? salaryStats.Max() : 0;

            // Pass statistics to view
            ViewBag.TotalProfiles = totalProfiles;
            ViewBag.ApprovedProfiles = approvedProfiles;
            ViewBag.PendingProfiles = pendingProfiles;
            ViewBag.RejectedProfiles = rejectedProfiles;
            ViewBag.ProfilesByCategory = profilesByCategory;
            ViewBag.ProfilesByLocation = profilesByLocation;
            ViewBag.AverageSalary = averageSalary;
            ViewBag.MinSalary = minSalary;
            ViewBag.MaxSalary = maxSalary;

            return View(recentProfiles);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveProfile(int profileId)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.ProfileId == profileId);

                if (profile != null)
                {
                    // Get current admin user ID
                    var adminUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                    
                    profile.ApprovalStatus = "Approved";
                    profile.ApprovedBy = adminUserId;
                    profile.ApprovedDate = DateTime.UtcNow;
                    profile.RejectionReason = null;
                    profile.UpdatedDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    // Send notification to user
                    await _notificationService.CreateProfileApprovalNotificationAsync(
                        profile.UserId, 
                        profile.User.FullName, 
                        true
                    );

                    TempData["Success"] = $"Hồ sơ của {profile.User.FullName} đã được duyệt thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi duyệt hồ sơ: {ex.Message}";
            }
            return RedirectToAction(nameof(JobProfiles));
        }

        [HttpPost]
        public async Task<IActionResult> RejectProfile(int profileId, string rejectionReason)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.ProfileId == profileId);

                if (profile != null)
                {
                    // Get current admin user ID
                    var adminUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
                    
                    profile.ApprovalStatus = "Rejected";
                    profile.ApprovedBy = adminUserId;
                    profile.ApprovedDate = DateTime.UtcNow;
                    profile.RejectionReason = rejectionReason?.Trim();
                    profile.UpdatedDate = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    // Send notification to user
                    await _notificationService.CreateProfileApprovalNotificationAsync(
                        profile.UserId, 
                        profile.User.FullName, 
                        false, 
                        rejectionReason
                    );

                    TempData["Success"] = $"Hồ sơ của {profile.User.FullName} đã bị từ chối!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi từ chối hồ sơ: {ex.Message}";
            }
            return RedirectToAction(nameof(JobProfiles));
        }

        [HttpGet]
        public async Task<IActionResult> ProfileDetail(int id)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .Include(p => p.User)
                    .Include(p => p.ApprovedByUser)
                    .Include(p => p.WorkExperiences)
                        .ThenInclude(w => w.WorkType)
                    .Include(p => p.Degrees)
                        .ThenInclude(d => d.DegreeType)
                    .Include(p => p.Certificates)
                        .ThenInclude(c => c.CertificateType)
                    .Include(p => p.UserProfileSkills)
                        .ThenInclude(ups => ups.Skill)
                            .ThenInclude(s => s.SkillType)
                    .Include(p => p.UserProfileProfessions)
                        .ThenInclude(upp => upp.Profession)
                            .ThenInclude(pr => pr.Category)
                    .FirstOrDefaultAsync(p => p.ProfileId == id);

                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ!";
                    return RedirectToAction(nameof(JobProfiles));
                }

                return View(profile);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải chi tiết hồ sơ: {ex.Message}";
                return RedirectToAction(nameof(JobProfiles));
            }
        }

        // Job Seeker Statistics
        public async Task<IActionResult> JobSeekerStats(int? year, int? month)
        {
            var currentYear = year ?? DateTime.Now.Year;
            var currentMonth = month;

            // Base query
            var profilesQuery = _context.UserProfiles.AsQueryable();

            // Filter by year
            if (year.HasValue)
            {
                profilesQuery = profilesQuery.Where(p => p.CreatedDate.Year == year.Value);
            }

            // Filter by month if specified
            if (month.HasValue && year.HasValue)
            {
                profilesQuery = profilesQuery.Where(p => p.CreatedDate.Month == month.Value);
            }

            // Basic statistics
            var totalProfiles = await profilesQuery.CountAsync();
            var approvedProfiles = await profilesQuery.CountAsync(p => p.ApprovalStatus == "Approved");
            var pendingProfiles = await profilesQuery.CountAsync(p => p.ApprovalStatus == "Pending");
            var rejectedProfiles = await profilesQuery.CountAsync(p => p.ApprovalStatus == "Rejected");

            // Monthly statistics for the current year
            var monthlyStats = await _context.UserProfiles
                .Where(p => p.CreatedDate.Year == currentYear)
                .GroupBy(p => p.CreatedDate.Month)
                .Select(g => new { 
                    Month = g.Key, 
                    Total = g.Count(),
                    Approved = g.Count(p => p.ApprovalStatus == "Approved"),
                    Pending = g.Count(p => p.ApprovalStatus == "Pending"),
                    Rejected = g.Count(p => p.ApprovalStatus == "Rejected")
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            // Yearly statistics
            var yearlyStats = await _context.UserProfiles
                .GroupBy(p => p.CreatedDate.Year)
                .Select(g => new { 
                    Year = g.Key, 
                    Total = g.Count(),
                    Approved = g.Count(p => p.ApprovalStatus == "Approved"),
                    Pending = g.Count(p => p.ApprovalStatus == "Pending"),
                    Rejected = g.Count(p => p.ApprovalStatus == "Rejected")
                })
                .OrderByDescending(x => x.Year)
                .Take(5)
                .ToListAsync();

            // Profiles by category (filtered)
            var profilesByCategory = await profilesQuery
                .Where(p => p.ApprovalStatus == "Approved")
                .SelectMany(p => p.UserProfileProfessions)
                .GroupBy(upp => upp.Profession.Category.CategoryName)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            // Profiles by location (filtered)
            var profilesByLocation = await profilesQuery
                .Where(p => p.ApprovalStatus == "Approved" && !string.IsNullOrEmpty(p.Location))
                .GroupBy(p => p.Location)
                .Select(g => new { Location = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToListAsync();

            // Salary statistics (filtered)
            var salaryProfiles = await profilesQuery
                .Where(p => p.ApprovalStatus == "Approved" && p.ExpectedSalary.HasValue)
                .Select(p => p.ExpectedSalary.Value)
                .ToListAsync();

            var averageSalary = salaryProfiles.Any() ? salaryProfiles.Average() : 0;
            var minSalary = salaryProfiles.Any() ? salaryProfiles.Min() : 0;
            var maxSalary = salaryProfiles.Any() ? salaryProfiles.Max() : 0;

            // Available years for filter
            var availableYears = await _context.UserProfiles
                .Select(p => p.CreatedDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var stats = new
            {
                TotalProfiles = totalProfiles,
                ApprovedProfiles = approvedProfiles,
                PendingProfiles = pendingProfiles,
                RejectedProfiles = rejectedProfiles,
                ProfilesByCategory = profilesByCategory,
                ProfilesByLocation = profilesByLocation,
                AverageSalary = averageSalary,
                MinSalary = minSalary,
                MaxSalary = maxSalary,
                MonthlyStats = monthlyStats,
                YearlyStats = yearlyStats,
                AvailableYears = availableYears,
                CurrentYear = currentYear,
                CurrentMonth = currentMonth
            };

            ViewBag.Stats = stats;
            return View();
        }

        // Work Types Management
        public async Task<IActionResult> WorkTypes()
        {
            var workTypes = await _context.WorkTypes
                .Include(wt => wt.WorkExperiences)
                .ToListAsync();
            return View(workTypes);
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkType(string workTypeName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(workTypeName))
                {
                    var workType = new WorkType
                    {
                        WorkTypeName = workTypeName
                    };
                    _context.WorkTypes.Add(workType);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại hình công việc đã được tạo thành công!";
                }
                else
                {
                    TempData["Error"] = "Tên loại hình công việc không được để trống!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tạo loại hình công việc: {ex.Message}";
            }
            return RedirectToAction(nameof(WorkTypes));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWorkType(int workTypeId, string workTypeName)
        {
            try
            {
                var workType = await _context.WorkTypes.FindAsync(workTypeId);
                
                if (workType != null && !string.IsNullOrWhiteSpace(workTypeName))
                {
                    workType.WorkTypeName = workTypeName;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Loại hình công việc đã được cập nhật thành công!";
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại hình công việc hoặc tên loại hình công việc không hợp lệ!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi cập nhật loại hình công việc: {ex.Message}";
            }
            return RedirectToAction(nameof(WorkTypes));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWorkType(int workTypeId)
        {
            try
            {
                var workType = await _context.WorkTypes
                    .Include(wt => wt.WorkExperiences)
                    .FirstOrDefaultAsync(wt => wt.WorkTypeId == workTypeId);
                
                if (workType != null)
                {
                    if (workType.WorkExperiences.Any())
                    {
                        TempData["Error"] = "Không thể xóa loại hình công việc này vì còn có kinh nghiệm liên quan!";
                    }
                    else
                    {
                        _context.WorkTypes.Remove(workType);
                        await _context.SaveChangesAsync();
                        TempData["Success"] = "Loại hình công việc đã được xóa thành công!";
                    }
                }
                else
                {
                    TempData["Error"] = "Không tìm thấy loại hình công việc!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa loại hình công việc: {ex.Message}";
            }
            return RedirectToAction(nameof(WorkTypes));
        }


        // Work Experiences Overview
        public async Task<IActionResult> WorkExperiences()
        {
            var workExperiences = await _context.WorkExperiences
                .Include(w => w.Profile)
                    .ThenInclude(p => p.User)
                .Include(w => w.WorkType)
                .OrderByDescending(w => w.StartDate)
                .ToListAsync();
            return View(workExperiences);
        }

        // Clean Duplicate Data
        [HttpPost]
        public async Task<IActionResult> CleanDuplicateData()
        {
            try
            {
                var duplicatesRemoved = 0;

                // Clean duplicate work experiences
                var workExperiences = await _context.WorkExperiences
                    .GroupBy(w => new { w.ProfileId, WorkName = w.WorkName.ToLower(), CompanyName = w.CompanyName.ToLower() })
                    .Where(g => g.Count() > 1)
                    .ToListAsync();

                foreach (var group in workExperiences)
                {
                    var duplicates = group.OrderBy(w => w.ExperienceId).Skip(1).ToList();
                    _context.WorkExperiences.RemoveRange(duplicates);
                    duplicatesRemoved += duplicates.Count;
                }

                // Clean duplicate degrees
                var degrees = await _context.Degrees
                    .GroupBy(d => new { d.ProfileId, DegreeName = d.DegreeName.ToLower(), SchoolName = d.SchoolName.ToLower() })
                    .Where(g => g.Count() > 1)
                    .ToListAsync();

                foreach (var group in degrees)
                {
                    var duplicates = group.OrderBy(d => d.DegreeId).Skip(1).ToList();
                    _context.Degrees.RemoveRange(duplicates);
                    duplicatesRemoved += duplicates.Count;
                }

                // Clean duplicate certificates
                var certificates = await _context.Certificates
                    .GroupBy(c => new { c.ProfileId, CertificateName = c.CertificateName.ToLower(), IssueBy = c.IssueBy.ToLower() })
                    .Where(g => g.Count() > 1)
                    .ToListAsync();

                foreach (var group in certificates)
                {
                    var duplicates = group.OrderBy(c => c.CertificateId).Skip(1).ToList();
                    _context.Certificates.RemoveRange(duplicates);
                    duplicatesRemoved += duplicates.Count;
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = $"Đã xóa thành công {duplicatesRemoved} bản ghi trùng lặp!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi xóa dữ liệu trùng lặp: {ex.Message}";
            }

            return RedirectToAction(nameof(JobProfiles));
        }

        // Test notification action
        [HttpPost]
        public async Task<IActionResult> SendTestNotification()
        {
            try
            {
                // Send test notification to all users
                await _notificationService.CreateSystemNotificationAsync(
                    "Thông báo hệ thống",
                    "Đây là thông báo test từ hệ thống. Tính năng thông báo đã hoạt động!",
                    "Info"
                );

                TempData["Success"] = "Đã gửi thông báo test đến tất cả người dùng!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi gửi thông báo: {ex.Message}";
            }
            return RedirectToAction(nameof(JobProfiles));
        }
    }
}