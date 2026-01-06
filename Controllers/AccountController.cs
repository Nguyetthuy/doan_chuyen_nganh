using finder_work.Data;
using finder_work.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace finder_work.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AccountController(ApplicationDbContext db)
        {
            _db = db;
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            try
            {
                Console.WriteLine($"=== Login Attempt ===");
                Console.WriteLine($"Email: {email}");
                Console.WriteLine($"Password length: {(password?.Length).GetValueOrDefault()}");
                
                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("ERROR: Email or password is empty");
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ email và mật khẩu");
                    return View();
                }

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
                Console.WriteLine($"User found: {user != null}");
                
                if (user == null)
                {
                    Console.WriteLine("ERROR: User not found");
                    ModelState.AddModelError(string.Empty, "Sai thông tin đăng nhập");
                    return View();
                }

                Console.WriteLine($"User details - ID: {user.UserId}, Role: {user.Role}, FullName: {user.FullName}");
                Console.WriteLine($"Stored password hash: {user.Password?.Substring(0, Math.Min(20, user.Password?.Length ?? 0))}...");

                var ok = false;
                if (!string.IsNullOrEmpty(user.Password))
                {
                    // Lưu ý: cột Password sẽ chứa giá trị băm
                    ok = PasswordUtils.Verify(password, user.Password);
                    Console.WriteLine($"Password verification result: {ok}");
                }

                if (!ok)
                {
                    Console.WriteLine("ERROR: Password verification failed");
                    ModelState.AddModelError(string.Empty, "Sai thông tin đăng nhập");
                    return View();
                }

                Console.WriteLine("Creating claims...");
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName ?? user.Email ?? "User"),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, user.Role ?? "User")
                };
                
                Console.WriteLine($"Claims created: {claims.Count}");
                foreach (var claim in claims)
                {
                    Console.WriteLine($"  - {claim.Type}: {claim.Value}");
                }

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                
                Console.WriteLine("Signing in user...");
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                Console.WriteLine("Sign in completed");

                if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Redirecting to Admin Dashboard");
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }

                Console.WriteLine("Redirecting to Home");
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CRITICAL ERROR in Login: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra trong quá trình đăng nhập. Vui lòng thử lại.");
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string fullName, string email, string phone, string password, string confirmPassword)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu không khớp");
                return View();
            }

            var exists = await _db.Users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                ModelState.AddModelError(string.Empty, "Email đã tồn tại");
                return View();
            }

            var user = new User
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phone,
                Role = "User",
                Password = PasswordUtils.Hash(password)
            };
            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
