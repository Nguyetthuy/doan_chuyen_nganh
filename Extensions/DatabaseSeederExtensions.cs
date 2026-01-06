using finder_work.Data;
using finder_work.Models;
using Microsoft.EntityFrameworkCore;

namespace finder_work.Extensions
{
    public static class DatabaseSeederExtensions
    {
        public static async Task SeedReferenceDataAsync(this ApplicationDbContext context)
        {
            try
            {
                // Chỉ tạo admin user, không tạo dữ liệu mẫu khác
                // Admin đã tự thêm các danh mục cần thiết
                Console.WriteLine("Database seeding completed - no sample data created as admin has already added categories");
            }
            catch (Exception ex)
            {
                // Log the error but don't throw to prevent application startup failure
                Console.WriteLine($"Error in seeding process: {ex.Message}");
            }
        }
    }
}