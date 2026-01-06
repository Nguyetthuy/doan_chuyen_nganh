using finder_work.Data;
using finder_work.Models;
using finder_work.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Configure services using extension methods for better organization
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddAuthenticationServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "categoryProfiles",
    pattern: "Categories/Profiles/{id:int}",
    defaults: new { controller = "Categories", action = "Profiles" });

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    
    try
    {
        // Ensure database is created
        await db.Database.EnsureCreatedAsync();
        
        // Seed reference data for degree types and certificate types
        await db.SeedReferenceDataAsync();
        
        // Create default admin user
        var adminEmail = "admin@gmail.com";
        var admin = await db.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);
        if (admin == null)
        {
            admin = new User
            {
                FullName = "Administrator",
                Email = adminEmail,
                Role = "Admin",
                Password = PasswordUtils.Hash("11111111")
            };
            db.Users.Add(admin);
            Console.WriteLine("Creating admin user...");
        }
        
        // Create test user
        var userEmail = "user@gmail.com";
        var testUser = await db.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
        if (testUser == null)
        {
            testUser = new User
            {
                FullName = "Test User",
                Email = userEmail,
                Role = "User",
                Password = PasswordUtils.Hash("11111111")
            };
            db.Users.Add(testUser);
            Console.WriteLine("Creating test user...");
        }
        
        await db.SaveChangesAsync();
        Console.WriteLine("Database seeding completed successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during database seeding: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

app.Run();
