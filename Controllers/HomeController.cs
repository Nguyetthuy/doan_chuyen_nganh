using System.Diagnostics;
using finder_work.Data;
using finder_work.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finder_work.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 6;
            
            // Validate page parameter
            page = Math.Max(1, page);
            
            // Get approved profiles for homepage display with pagination
            var totalProfiles = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved")
                .CountAsync();
                
            var totalPages = (int)Math.Ceiling((double)totalProfiles / pageSize);
            
            // Ensure page is within valid range
            if (page > totalPages && totalPages > 0)
                page = totalPages;
            
            var approvedProfiles = await _context.UserProfiles
                .Include(p => p.User)
                .Include(p => p.UserProfileSkills)
                    .ThenInclude(ups => ups.Skill)
                .Include(p => p.UserProfileProfessions)
                    .ThenInclude(upp => upp.Profession)
                .Where(p => p.ApprovalStatus == "Approved")
                .OrderByDescending(p => p.UpdatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.RecentProfiles = approvedProfiles;
            
            // Pagination info
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProfiles = totalProfiles;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.PageSize = pageSize;
            
            // Get statistics for homepage
            ViewBag.TotalApprovedProfiles = totalProfiles;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Search(string keyword, string location, int? categoryId, 
            decimal? minSalary, decimal? maxSalary, int page = 1)
        {
            const int pageSize = 12;
            
            var query = _context.UserProfiles
                .Include(p => p.User)
                .Include(p => p.UserProfileSkills)
                    .ThenInclude(ups => ups.Skill)
                .Include(p => p.UserProfileProfessions)
                    .ThenInclude(upp => upp.Profession)
                        .ThenInclude(pr => pr.Category)
                .Where(p => p.ApprovalStatus == "Approved");

            // Filter by keyword (search in user name, skills, professions, summary)
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var keywordLower = keyword.ToLower();
                query = query.Where(p => 
                    (p.User.FullName != null && p.User.FullName.ToLower().Contains(keywordLower)) ||
                    (p.Summary != null && p.Summary.ToLower().Contains(keywordLower)) ||
                    p.UserProfileSkills.Any(ups => ups.Skill.SkillName != null && ups.Skill.SkillName.ToLower().Contains(keywordLower)) ||
                    p.UserProfileProfessions.Any(upp => upp.Profession.ProfessionName != null && upp.Profession.ProfessionName.ToLower().Contains(keywordLower))
                );
            }

            // Filter by location
            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(p => p.Location != null && p.Location.ToLower().Contains(location.ToLower()));
            }

            // Filter by category
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.UserProfileProfessions
                    .Any(upp => upp.Profession.CategoryId == categoryId.Value));
            }

            // Filter by salary range
            if (minSalary.HasValue)
            {
                query = query.Where(p => p.ExpectedSalary >= minSalary.Value);
            }

            if (maxSalary.HasValue)
            {
                query = query.Where(p => p.ExpectedSalary <= maxSalary.Value);
            }

            // Get total count for pagination
            var totalResults = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalResults / pageSize);
            
            // Validate page parameter
            page = Math.Max(1, page);
            if (page > totalPages && totalPages > 0)
                page = totalPages;

            var results = await query
                .OrderByDescending(p => p.UpdatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass search parameters and pagination info to view
            ViewBag.SearchParams = new
            {
                Keyword = keyword,
                Location = location,
                CategoryId = categoryId,
                MinSalary = minSalary,
                MaxSalary = maxSalary
            };
            
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalResults = totalResults;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.PageSize = pageSize;

            // Get category name for display
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                var category = await _context.Categories.FindAsync(categoryId.Value);
                ViewBag.CategoryName = category?.CategoryName;
            }

            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> ProfileDetail(int id)
        {
            var profile = await _context.UserProfiles
                .Include(p => p.User)
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
                .FirstOrDefaultAsync(p => p.ProfileId == id && p.ApprovalStatus == "Approved");

            if (profile == null)
            {
                return NotFound();
            }

            // Use CVView instead of ProfileDetail to show CV format
            return View("~/Views/Profile/CVView.cshtml", profile);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetFeaturedProfiles(int page = 1)
        {
            const int pageSize = 6;
            
            // Validate page parameter
            page = Math.Max(1, page);
            
            var totalProfiles = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved")
                .CountAsync();
                
            var totalPages = (int)Math.Ceiling((double)totalProfiles / pageSize);
            
            // Ensure page is within valid range
            if (page > totalPages && totalPages > 0)
                page = totalPages;
            
            var profiles = await _context.UserProfiles
                .Include(p => p.User)
                .Include(p => p.UserProfileSkills)
                    .ThenInclude(ups => ups.Skill)
                .Include(p => p.UserProfileProfessions)
                    .ThenInclude(upp => upp.Profession)
                        .ThenInclude(pr => pr.Category)
                .Where(p => p.ApprovalStatus == "Approved")
                .OrderByDescending(p => p.UpdatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass pagination info to the view
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProfiles = totalProfiles;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;

            return PartialView("_FeaturedProfiles", profiles);
        }

        [HttpGet]
        public async Task<JsonResult> GetStatistics()
        {
            var stats = new
            {
                totalProfiles = await _context.UserProfiles.CountAsync(p => p.ApprovalStatus == "Approved"),
                totalCategories = await _context.Categories.CountAsync(),
                totalSkills = await _context.Skills.CountAsync(),
                totalLocations = await _context.UserProfiles
                    .Where(p => p.ApprovalStatus == "Approved" && !string.IsNullOrEmpty(p.Location))
                    .Select(p => p.Location)
                    .Distinct()
                    .CountAsync()
            };

            return Json(stats);
        }

        [HttpGet]
        public async Task<JsonResult> GetAllCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .OrderBy(c => c.CategoryName)
                    .Select(c => new
                    {
                        categoryId = c.CategoryId,
                        categoryName = c.CategoryName
                    })
                    .ToListAsync();

                return Json(categories);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCategoriesWithProfiles()
        {
            try
            {
                var categoriesWithCounts = await _context.Categories
                    .Select(c => new
                    {
                        categoryId = c.CategoryId,
                        categoryName = c.CategoryName,
                        categoryDescription = c.CategoryDescription,
                        profileCount = _context.UserProfiles
                            .Where(p => p.ApprovalStatus == "Approved")
                            .Where(p => p.UserProfileProfessions
                                .Any(upp => upp.Profession.CategoryId == c.CategoryId))
                            .Count()
                    })
                    .Where(x => x.profileCount > 0)
                    .OrderByDescending(x => x.profileCount)
                    .ToListAsync();

                return Json(categoriesWithCounts);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
