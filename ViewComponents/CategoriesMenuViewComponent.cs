using finder_work.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace finder_work.ViewComponents
{
    public class CategoriesMenuViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _context;

        public CategoriesMenuViewComponent(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categoriesWithCounts = await _context.Categories
                .Select(c => new
                {
                    Category = c,
                    ApprovedProfileCount = _context.UserProfiles
                        .Where(p => p.ApprovalStatus == "Approved")
                        .Where(p => p.UserProfileProfessions
                            .Any(upp => upp.Profession.CategoryId == c.CategoryId))
                        .Count()
                })
                .Where(x => x.ApprovedProfileCount > 0)
                .OrderBy(x => x.Category.CategoryName)
                .ToListAsync();

            return View(categoriesWithCounts);
        }
    }
}