using finder_work.Data;
using finder_work.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finder_work.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var categoriesWithCounts = await _context.Categories
                .Select(c => new
                {
                    Category = c,
                    ProfileCount = _context.UserProfiles
                        .Where(p => p.ApprovalStatus == "Approved")
                        .Where(p => p.UserProfileProfessions
                            .Any(upp => upp.Profession.CategoryId == c.CategoryId))
                        .Count()
                })
                .Where(x => x.ProfileCount > 0)
                .OrderByDescending(x => x.ProfileCount)
                .ToListAsync();

            return View(categoriesWithCounts);
        }

        public async Task<IActionResult> Profiles(int id, string categoryName, int page = 1)
        {
            const int pageSize = 6;
            
            // Validate page parameter
            page = Math.Max(1, page);
            
            // Get category info
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Get total profiles for this category
            var totalProfiles = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved")
                .Where(p => p.UserProfileProfessions
                    .Any(upp => upp.Profession.CategoryId == id))
                .CountAsync();
                
            var totalPages = (int)Math.Ceiling((double)totalProfiles / pageSize);
            
            // Ensure page is within valid range
            if (page > totalPages && totalPages > 0)
                page = totalPages;
            
            // Get profiles for this category with pagination
            var profiles = await _context.UserProfiles
                .Include(p => p.User)
                .Include(p => p.UserProfileSkills)
                    .ThenInclude(ups => ups.Skill)
                .Include(p => p.UserProfileProfessions)
                    .ThenInclude(upp => upp.Profession)
                        .ThenInclude(pr => pr.Category)
                .Where(p => p.ApprovalStatus == "Approved")
                .Where(p => p.UserProfileProfessions
                    .Any(upp => upp.Profession.CategoryId == id))
                .OrderByDescending(p => p.UpdatedDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Pass data to view
            ViewBag.Category = category;
            ViewBag.CategoryName = categoryName ?? category.CategoryName;
            ViewBag.CategoryId = id;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalProfiles = totalProfiles;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.PageSize = pageSize;

            return View(profiles);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoryProfiles(int id, int page = 1)
        {
            const int pageSize = 6;
            
            // Validate page parameter
            page = Math.Max(1, page);
            
            var totalProfiles = await _context.UserProfiles
                .Where(p => p.ApprovalStatus == "Approved")
                .Where(p => p.UserProfileProfessions
                    .Any(upp => upp.Profession.CategoryId == id))
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
                .Where(p => p.UserProfileProfessions
                    .Any(upp => upp.Profession.CategoryId == id))
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
            ViewBag.CategoryId = id;

            return PartialView("_CategoryProfiles", profiles);
        }
    }
}