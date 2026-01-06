using finder_work.Data;
using finder_work.Models;
using finder_work.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IO;

namespace finder_work.Controllers
{
    [Authorize(Roles = "User")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public ProfileController(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpGet]
        public IActionResult Test()
        {
            return Json(new { message = "ProfileController is working", userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value });
        }

        [HttpGet]
        public async Task<IActionResult> TestDelete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var profile = await _context.UserProfiles
                    .Include(p => p.UserProfileSkills)
                    .Include(p => p.UserProfileProfessions)
                    .Include(p => p.WorkExperiences)
                    .Include(p => p.Degrees)
                    .Include(p => p.Certificates)
                    .Include(p => p.CVs)
                    .FirstOrDefaultAsync(p => p.ProfileId == id && p.UserId == userId);

                if (profile == null)
                {
                    return Json(new { error = "Profile not found", profileId = id, userId = userId });
                }

                var debugInfo = new
                {
                    profileId = profile.ProfileId,
                    userId = profile.UserId,
                    currentUserId = userId,
                    skillsCount = profile.UserProfileSkills.Count,
                    professionsCount = profile.UserProfileProfessions.Count,
                    workExperiencesCount = profile.WorkExperiences.Count,
                    degreesCount = profile.Degrees.Count,
                    certificatesCount = profile.Certificates.Count,
                    cvsCount = profile.CVs.Count,
                    canDelete = profile.UserId == userId
                };

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                var profiles = await _context.UserProfiles
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
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.CreatedDate)
                    .ToListAsync();

                // Load dropdown data for modals
                try
                {
                    ViewBag.WorkTypes = await _context.WorkTypes
                        .Select(w => new { w.WorkTypeId, w.WorkTypeName })
                        .ToListAsync();
                    ViewBag.DegreeTypes = await _context.DegreeTypes
                        .Select(d => new { d.DegreeTypeId, d.DegreeTypeName })
                        .ToListAsync();
                    ViewBag.CertificateTypes = await _context.CertificateTypes
                        .Select(c => new { c.CertificateTypeId, c.CertificateTypeName })
                        .ToListAsync();
                }
                catch (Exception)
                {
                    ViewBag.WorkTypes = new List<object>();
                    ViewBag.DegreeTypes = new List<object>();
                    ViewBag.CertificateTypes = new List<object>();
                    
                    TempData["Warning"] = "Một số dữ liệu dropdown không thể tải. Vui lòng liên hệ admin nếu vấn đề tiếp tục.";
                }

                return View(profiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Profile Index: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Có lỗi xảy ra khi tải hồ sơ. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                // Get current user information
                var currentUser = await _context.Users.FindAsync(userId);
                if (currentUser == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Index", "Home");
                }
                
                ViewBag.CurrentUser = currentUser;

                // Load dropdown data with error handling
                try
                {
                    ViewBag.SkillTypes = await _context.SkillTypes
                        .Select(st => new { st.SkillTypeId, st.SkillTypeName })
                        .ToListAsync();
                    ViewBag.Categories = await _context.Categories
                        .Select(c => new { c.CategoryId, c.CategoryName })
                        .ToListAsync();
                    ViewBag.WorkTypes = await _context.WorkTypes
                        .Select(w => new { w.WorkTypeId, w.WorkTypeName })
                        .ToListAsync();
                    ViewBag.DegreeTypes = await _context.DegreeTypes
                        .Select(d => new { d.DegreeTypeId, d.DegreeTypeName })
                        .ToListAsync();
                    ViewBag.CertificateTypes = await _context.CertificateTypes
                        .Select(c => new { c.CertificateTypeId, c.CertificateTypeName })
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    ViewBag.SkillTypes = new List<object>();
                    ViewBag.Categories = new List<object>();
                    ViewBag.WorkTypes = new List<object>();
                    ViewBag.DegreeTypes = new List<object>();
                    ViewBag.CertificateTypes = new List<object>();
                    
                    TempData["Warning"] = "Một số dữ liệu dropdown không thể tải. Vui lòng liên hệ admin nếu vấn đề tiếp tục.";
                }

                return View();
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải trang tạo hồ sơ. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DebugCreate(UserProfile profile, string fullName, string phoneNumber, IFormFile avatar)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                // Debug form data
                var skillNames = Request.Form["skillNames"].ToList();
                var skillTypeIds = Request.Form["skillTypeIds"].ToList();
                var professionNames = Request.Form["professionNames"].ToList();
                var categoryIds = Request.Form["categoryIds"].ToList();

                var debugInfo = new
                {
                    userId = userId,
                    fullName = fullName,
                    phoneNumber = phoneNumber,
                    summary = profile.Summary,
                    location = profile.Location,
                    expectedSalary = profile.ExpectedSalary,
                    skillNames = skillNames,
                    skillTypeIds = skillTypeIds,
                    professionNames = professionNames,
                    categoryIds = categoryIds,
                    skillNamesCount = skillNames.Count,
                    skillTypeIdsCount = skillTypeIds.Count,
                    professionNamesCount = professionNames.Count,
                    categoryIdsCount = categoryIds.Count,
                    allFormKeys = Request.Form.Keys.ToList()
                };

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfile profile, string fullName, string phoneNumber, IFormFile avatar)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                // Comprehensive validation
                var validationErrors = new List<string>();

                if (string.IsNullOrWhiteSpace(fullName))
                    validationErrors.Add("Vui lòng nhập họ và tên đầy đủ!");

                if (string.IsNullOrWhiteSpace(phoneNumber))
                    validationErrors.Add("Vui lòng nhập số điện thoại!");

                if (string.IsNullOrWhiteSpace(profile.Summary))
                    validationErrors.Add("Vui lòng nhập tóm tắt về bản thân!");

                if (string.IsNullOrWhiteSpace(profile.Location))
                    validationErrors.Add("Vui lòng nhập địa điểm làm việc mong muốn!");

                // Check if at least one skill is selected
                var inputSkillNames = Request.Form["skillNames"].ToList();
                if (!inputSkillNames.Any() || inputSkillNames.All(s => string.IsNullOrWhiteSpace(s)))
                    validationErrors.Add("Vui lòng thêm ít nhất 1 kỹ năng!");

                // Check if at least one profession is selected
                var inputProfessionNames = Request.Form["professionNames"].ToList();
                if (!inputProfessionNames.Any() || inputProfessionNames.All(p => string.IsNullOrWhiteSpace(p)))
                    validationErrors.Add("Vui lòng thêm ít nhất 1 ngành nghề!");

                if (validationErrors.Any())
                {
                    TempData["Error"] = string.Join("<br/>", validationErrors);
                    return await ReloadCreateView();
                }

                // Update user information first
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.FullName = fullName.Trim();
                    user.PhoneNumber = phoneNumber.Trim();

                    // Handle avatar upload
                    if (avatar != null && avatar.Length > 0)
                    {
                        try
                        {
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var fileExtension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
                            
                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                TempData["Warning"] = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif). Hồ sơ sẽ được tạo không có ảnh đại diện.";
                            }
                            else if (avatar.Length > 5 * 1024 * 1024) // 5MB limit
                            {
                                TempData["Warning"] = "File ảnh quá lớn (tối đa 5MB). Hồ sơ sẽ được tạo không có ảnh đại diện.";
                            }
                            else
                            {
                                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                                Directory.CreateDirectory(uploadsFolder);
                                
                                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await avatar.CopyToAsync(fileStream);
                                }
                                
                                user.Avata = $"/uploads/avatars/{uniqueFileName}";
                            }
                        }
                        catch (Exception ex)
                        {
                            TempData["Warning"] = "Không thể tải lên ảnh đại diện. Hồ sơ sẽ được tạo không có ảnh đại diện.";
                        }
                    }

                    _context.Users.Update(user);
                }

                // Create profile
                profile.UserId = userId;
                profile.ApprovalStatus = "Pending";
                profile.CreatedDate = DateTime.UtcNow;
                profile.UpdatedDate = DateTime.UtcNow;

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                // Process skills from form data - Create new skills with user input
                var skillNames = Request.Form["skillNames"].ToList();
                var skillTypeIds = Request.Form["skillTypeIds"].ToList();
                
                var skillsToAdd = new List<UserProfileSkill>();
                
                for (int i = 0; i < skillNames.Count && i < skillTypeIds.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(skillNames[i]) && int.TryParse(skillTypeIds[i], out var skillTypeId) && skillTypeId > 0)
                    {
                        try
                        {
                            // Check if skill already exists
                            var existingSkill = await _context.Skills
                                .FirstOrDefaultAsync(s => s.SkillName.ToLower() == skillNames[i].Trim().ToLower() && s.SkillTypeId == skillTypeId);
                            
                            if (existingSkill == null)
                            {
                                // Create new skill
                                existingSkill = new Skill
                                {
                                    SkillName = skillNames[i].Trim(),
                                    SkillTypeId = skillTypeId
                                };
                                _context.Skills.Add(existingSkill);
                                await _context.SaveChangesAsync(); // Save to get SkillId
                            }
                            
                            // Add to list to be saved later
                            skillsToAdd.Add(new UserProfileSkill
                            {
                                ProfileId = profile.ProfileId,
                                SkillId = existingSkill.SkillId
                            });
                        }
                        catch (Exception ex)
                        {
                            // Log error but continue processing other skills
                            Console.WriteLine($"Error processing skill {skillNames[i]}: {ex.Message}");
                        }
                    }
                }

                // Process professions from form data - Create new professions with user input
                var professionNames = Request.Form["professionNames"].ToList();
                var categoryIds = Request.Form["categoryIds"].ToList();
                
                var professionsToAdd = new List<UserProfileProfession>();
                
                for (int i = 0; i < professionNames.Count && i < categoryIds.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(professionNames[i]) && int.TryParse(categoryIds[i], out var categoryId) && categoryId > 0)
                    {
                        try
                        {
                            // Check if profession already exists
                            var existingProfession = await _context.Professions
                                .FirstOrDefaultAsync(p => p.ProfessionName.ToLower() == professionNames[i].Trim().ToLower() && p.CategoryId == categoryId);
                            
                            if (existingProfession == null)
                            {
                                // Create new profession
                                existingProfession = new Profession
                                {
                                    ProfessionName = professionNames[i].Trim(),
                                    CategoryId = categoryId
                                };
                                _context.Professions.Add(existingProfession);
                                await _context.SaveChangesAsync(); // Save to get ProfessionId
                            }
                            
                            // Add to list to be saved later
                            professionsToAdd.Add(new UserProfileProfession
                            {
                                ProfileId = profile.ProfileId,
                                ProfessionId = existingProfession.ProfessionId
                            });
                        }
                        catch (Exception ex)
                        {
                            // Log error but continue processing other professions
                            Console.WriteLine($"Error processing profession {professionNames[i]}: {ex.Message}");
                        }
                    }
                }

                // Process work experiences, degrees, and certificates
                await ProcessWorkExperiences(profile.ProfileId);
                await ProcessDegrees(profile.ProfileId, userId);
                await ProcessCertificates(profile.ProfileId, userId);

                // Add skills and professions to context
                if (skillsToAdd.Any())
                {
                    _context.UserProfileSkills.AddRange(skillsToAdd);
                }
                
                if (professionsToAdd.Any())
                {
                    _context.UserProfileProfessions.AddRange(professionsToAdd);
                }

                await _context.SaveChangesAsync();

                // Send notification to user about profile creation
                await _notificationService.CreateNotificationAsync(
                    userId,
                    "Hồ sơ đã được tạo thành công",
                    "Hồ sơ của bạn đã được tạo và đang chờ admin duyệt. Bạn sẽ nhận được thông báo khi hồ sơ được duyệt.",
                    "ProfilePending",
                    "/Profile",
                    "Xem hồ sơ"
                );

                TempData["Success"] = "Hồ sơ đã được tạo thành công với đầy đủ thông tin! Hồ sơ đang chờ admin duyệt.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException)
            {
                TempData["Error"] = "Có lỗi xảy ra khi lưu dữ liệu. Vui lòng kiểm tra thông tin và thử lại.";
                return await ReloadCreateView();
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi không mong muốn xảy ra. Vui lòng thử lại sau.";
                return await ReloadCreateView();
            }
        }

        private async Task ProcessWorkExperiences(int profileId)
        {
            var workExperienceKeys = Request.Form.Keys.Where(k => k.StartsWith("workExperiences[") && k.EndsWith("].WorkName")).ToList();
            var processedWorkExperiences = new HashSet<string>(); // Để tránh trùng lặp
            
            foreach (var key in workExperienceKeys)
            {
                var index = key.Substring("workExperiences[".Length, key.IndexOf(']') - "workExperiences[".Length);
                
                var workName = Request.Form[$"workExperiences[{index}].WorkName"].ToString().Trim();
                var companyName = Request.Form[$"workExperiences[{index}].CompanyName"].ToString().Trim();
                
                if (!string.IsNullOrWhiteSpace(workName) && !string.IsNullOrWhiteSpace(companyName))
                {
                    // Tạo key để kiểm tra trùng lặp
                    var workExpKey = $"{workName.ToLower()}|{companyName.ToLower()}";
                    
                    // Bỏ qua nếu đã xử lý kinh nghiệm này rồi
                    if (processedWorkExperiences.Contains(workExpKey))
                        continue;
                    
                    processedWorkExperiences.Add(workExpKey);

                    var workExp = new WorkExperience
                    {
                        ProfileId = profileId,
                        WorkName = workName,
                        CompanyName = companyName,
                        WorkDescription = Request.Form[$"workExperiences[{index}].WorkDescription"].ToString().Trim(),
                        WorkTypeId = null
                    };

                    if (int.TryParse(Request.Form[$"workExperiences[{index}].WorkTypeId"], out var workTypeId) && workTypeId > 0)
                    {
                        workExp.WorkTypeId = workTypeId;
                    }

                    if (DateTime.TryParse(Request.Form[$"workExperiences[{index}].StartDate"], out var startDate))
                        workExp.StartDate = startDate;

                    if (DateTime.TryParse(Request.Form[$"workExperiences[{index}].EndDate"], out var endDate))
                        workExp.EndDate = endDate;

                    _context.WorkExperiences.Add(workExp);
                }
            }
        }

        private async Task ProcessDegrees(int profileId, int userId)
        {
            var degreeKeys = Request.Form.Keys.Where(k => k.StartsWith("degrees[") && k.EndsWith("].DegreeName")).ToList();
            var processedDegrees = new HashSet<string>(); // Để tránh trùng lặp
            
            foreach (var key in degreeKeys)
            {
                var index = key.Substring("degrees[".Length, key.IndexOf(']') - "degrees[".Length);
                
                var degreeName = Request.Form[$"degrees[{index}].DegreeName"].ToString().Trim();
                var schoolName = Request.Form[$"degrees[{index}].SchoolName"].ToString().Trim();
                
                if (!string.IsNullOrWhiteSpace(degreeName) && !string.IsNullOrWhiteSpace(schoolName))
                {
                    // Tạo key để kiểm tra trùng lặp
                    var degreeKey = $"{degreeName.ToLower()}|{schoolName.ToLower()}";
                    
                    // Bỏ qua nếu đã xử lý bằng cấp này rồi
                    if (processedDegrees.Contains(degreeKey))
                        continue;
                    
                    processedDegrees.Add(degreeKey);

                    var degree = new Degree
                    {
                        ProfileId = profileId,
                        DegreeName = degreeName,
                        SchoolName = schoolName,
                        Major = Request.Form[$"degrees[{index}].Major"].ToString().Trim(),
                        DegreeTypeId = null
                    };

                    if (int.TryParse(Request.Form[$"degrees[{index}].DegreeTypeId"], out var degreeTypeId) && degreeTypeId > 0)
                    {
                        degree.DegreeTypeId = degreeTypeId;
                    }

                    if (int.TryParse(Request.Form[$"degrees[{index}].GraduationYear"], out var graduationYear))
                        degree.GraduationYear = graduationYear;

                    // Handle degree image upload
                    var degreeImageKey = $"degrees[{index}].DegreeImage";
                    if (Request.Form.Files[degreeImageKey] != null)
                    {
                        var degreeImage = Request.Form.Files[degreeImageKey];
                        if (degreeImage.Length > 0)
                        {
                            try
                            {
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                                var fileExtension = Path.GetExtension(degreeImage.FileName).ToLowerInvariant();
                                
                                if (allowedExtensions.Contains(fileExtension) && degreeImage.Length <= 10 * 1024 * 1024)
                                {
                                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "degrees");
                                    Directory.CreateDirectory(uploadsFolder);
                                    
                                    var uniqueFileName = $"{userId}_{profileId}_{Guid.NewGuid()}{Path.GetExtension(degreeImage.FileName)}";
                                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                    
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await degreeImage.CopyToAsync(fileStream);
                                    }
                                    
                                    degree.DegreeImg = $"/uploads/degrees/{uniqueFileName}";
                                }
                            }
                            catch (Exception)
                            {
                                // Handle degree image upload error
                            }
                        }
                    }

                    _context.Degrees.Add(degree);
                }
            }
        }

        private async Task ProcessCertificates(int profileId, int userId)
        {
            var certificateKeys = Request.Form.Keys.Where(k => k.StartsWith("certificates[") && k.EndsWith("].CertificateName")).ToList();
            var processedCertificates = new HashSet<string>(); // Để tránh trùng lặp
            
            foreach (var key in certificateKeys)
            {
                var index = key.Substring("certificates[".Length, key.IndexOf(']') - "certificates[".Length);
                
                var certificateName = Request.Form[$"certificates[{index}].CertificateName"].ToString().Trim();
                var issueBy = Request.Form[$"certificates[{index}].IssueBy"].ToString().Trim();
                
                if (!string.IsNullOrWhiteSpace(certificateName) && !string.IsNullOrWhiteSpace(issueBy))
                {
                    // Tạo key để kiểm tra trùng lặp
                    var certificateKey = $"{certificateName.ToLower()}|{issueBy.ToLower()}";
                    
                    // Bỏ qua nếu đã xử lý chứng chỉ này rồi
                    if (processedCertificates.Contains(certificateKey))
                        continue;
                    
                    processedCertificates.Add(certificateKey);

                    var certificate = new Certificate
                    {
                        ProfileId = profileId,
                        CertificateName = certificateName,
                        IssueBy = issueBy,
                        CertificateTypeId = 1 // Default certificate type
                    };

                    if (int.TryParse(Request.Form[$"certificates[{index}].CertificateTypeId"], out var certificateTypeId) && certificateTypeId > 0)
                    {
                        certificate.CertificateTypeId = certificateTypeId;
                    }

                    if (DateTime.TryParse(Request.Form[$"certificates[{index}].IssueDate"], out var issueDate))
                        certificate.IssueDate = issueDate;

                    if (DateTime.TryParse(Request.Form[$"certificates[{index}].ExpiryDate"], out var expiryDate))
                        certificate.ExpiryDate = expiryDate;

                    // Handle certificate image upload
                    var certificateImageKey = $"certificates[{index}].CertificateImage";
                    if (Request.Form.Files[certificateImageKey] != null)
                    {
                        var certificateImage = Request.Form.Files[certificateImageKey];
                        if (certificateImage.Length > 0)
                        {
                            try
                            {
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
                                var fileExtension = Path.GetExtension(certificateImage.FileName).ToLowerInvariant();
                                
                                if (allowedExtensions.Contains(fileExtension) && certificateImage.Length <= 10 * 1024 * 1024)
                                {
                                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "certificates");
                                    Directory.CreateDirectory(uploadsFolder);
                                    
                                    var uniqueFileName = $"{userId}_{profileId}_{Guid.NewGuid()}{Path.GetExtension(certificateImage.FileName)}";
                                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                    
                                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                                    {
                                        await certificateImage.CopyToAsync(fileStream);
                                    }
                                    
                                    certificate.CertificateImg = $"/uploads/certificates/{uniqueFileName}";
                                }
                            }
                            catch (Exception)
                            {
                                // Handle certificate image upload error
                            }
                        }
                    }

                    _context.Certificates.Add(certificate);
                }
            }
        }

        private async Task<IActionResult> ReloadCreateView()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var currentUser = await _context.Users.FindAsync(userId);
                
                if (currentUser == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Index", "Home");
                }
                
                ViewBag.CurrentUser = currentUser;

                // Load dropdown data with error handling
                try
                {
                    ViewBag.SkillTypes = await _context.SkillTypes
                        .Select(st => new { st.SkillTypeId, st.SkillTypeName })
                        .ToListAsync();
                    ViewBag.Categories = await _context.Categories
                        .Select(c => new { c.CategoryId, c.CategoryName })
                        .ToListAsync();
                    ViewBag.WorkTypes = await _context.WorkTypes
                        .Select(w => new { w.WorkTypeId, w.WorkTypeName })
                        .ToListAsync();
                    ViewBag.DegreeTypes = await _context.DegreeTypes
                        .Select(d => new { d.DegreeTypeId, d.DegreeTypeName })
                        .ToListAsync();
                    ViewBag.CertificateTypes = await _context.CertificateTypes
                        .Select(c => new { c.CertificateTypeId, c.CertificateTypeName })
                        .ToListAsync();
                }
                catch (Exception)
                {
                    ViewBag.SkillTypes = new List<object>();
                    ViewBag.Categories = new List<object>();
                    ViewBag.WorkTypes = new List<object>();
                    ViewBag.DegreeTypes = new List<object>();
                    ViewBag.CertificateTypes = new List<object>();
                    
                    TempData["Warning"] = "Một số dữ liệu dropdown không thể tải. Vui lòng liên hệ admin nếu vấn đề tiếp tục.";
                }

                return View("Create");
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải lại trang. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

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
                    .FirstOrDefaultAsync(p => p.ProfileId == id && p.UserId == userId);

                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ hoặc bạn không có quyền truy cập.";
                    return RedirectToAction(nameof(Index));
                }

                // Load dropdown data for modals
                ViewBag.WorkTypes = await _context.WorkTypes
                    .Select(w => new { w.WorkTypeId, w.WorkTypeName })
                    .ToListAsync();
                ViewBag.DegreeTypes = await _context.DegreeTypes
                    .Select(d => new { d.DegreeTypeId, d.DegreeTypeName })
                    .ToListAsync();
                ViewBag.CertificateTypes = await _context.CertificateTypes
                    .Select(c => new { c.CertificateTypeId, c.CertificateTypeName })
                    .ToListAsync();

                return View(profile);
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải chi tiết hồ sơ.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> SeedTestData()
        {
            try
            {
                // Tạo SkillTypes nếu chưa có
                if (!await _context.SkillTypes.AnyAsync())
                {
                    var skillTypes = new[]
                    {
                        new SkillType { SkillTypeName = "Kỹ năng kỹ thuật" },
                        new SkillType { SkillTypeName = "Kỹ năng mềm" },
                        new SkillType { SkillTypeName = "Ngôn ngữ lập trình" },
                        new SkillType { SkillTypeName = "Công cụ & Framework" }
                    };
                    _context.SkillTypes.AddRange(skillTypes);
                }

                // Tạo Categories nếu chưa có
                if (!await _context.Categories.AnyAsync())
                {
                    var categories = new[]
                    {
                        new Category { CategoryName = "Công nghệ thông tin", CategoryDescription = "IT và phần mềm" },
                        new Category { CategoryName = "Marketing", CategoryDescription = "Marketing và quảng cáo" },
                        new Category { CategoryName = "Kinh doanh", CategoryDescription = "Kinh doanh và bán hàng" },
                        new Category { CategoryName = "Thiết kế", CategoryDescription = "Thiết kế đồ họa và UI/UX" }
                    };
                    _context.Categories.AddRange(categories);
                }

                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Test data seeded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DebugFormData()
        {
            try
            {
                var formData = new Dictionary<string, object>();
                
                foreach (var key in Request.Form.Keys)
                {
                    formData[key] = Request.Form[key].ToArray();
                }

                var skillNames = Request.Form["skillNames"].ToList();
                var skillTypeIds = Request.Form["skillTypeIds"].ToList();
                var professionNames = Request.Form["professionNames"].ToList();
                var categoryIds = Request.Form["categoryIds"].ToList();

                return Json(new
                {
                    allFormData = formData,
                    skillNames = skillNames,
                    skillTypeIds = skillTypeIds,
                    professionNames = professionNames,
                    categoryIds = categoryIds,
                    skillNamesCount = skillNames.Count,
                    skillTypeIdsCount = skillTypeIds.Count,
                    professionNamesCount = professionNames.Count,
                    categoryIdsCount = categoryIds.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestCreateSkillProfession()
        {
            try
            {
                // Test tạo skill mới
                var testSkill = new Skill
                {
                    SkillName = "Test Skill " + DateTime.Now.Ticks,
                    SkillTypeId = 1 // Giả sử có SkillType với ID = 1
                };
                _context.Skills.Add(testSkill);
                await _context.SaveChangesAsync();

                // Test tạo profession mới
                var testProfession = new Profession
                {
                    ProfessionName = "Test Profession " + DateTime.Now.Ticks,
                    CategoryId = 1 // Giả sử có Category với ID = 1
                };
                _context.Professions.Add(testProfession);
                await _context.SaveChangesAsync();

                // Lấy profile đầu tiên để test
                var firstProfile = await _context.UserProfiles.FirstOrDefaultAsync();
                if (firstProfile == null)
                {
                    return Json(new { error = "No profile found to test with" });
                }

                // Test tạo UserProfileSkill
                var userProfileSkill = new UserProfileSkill
                {
                    ProfileId = firstProfile.ProfileId,
                    SkillId = testSkill.SkillId
                };
                _context.UserProfileSkills.Add(userProfileSkill);

                // Test tạo UserProfileProfession
                var userProfileProfession = new UserProfileProfession
                {
                    ProfileId = firstProfile.ProfileId,
                    ProfessionId = testProfession.ProfessionId
                };
                _context.UserProfileProfessions.Add(userProfileProfession);

                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    message = "Test data created successfully",
                    skillId = testSkill.SkillId,
                    professionId = testProfession.ProfessionId,
                    profileId = firstProfile.ProfileId
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    error = ex.Message,
                    innerException = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DebugDatabase()
        {
            try
            {
                var result = new
                {
                    SkillTypes = await _context.SkillTypes.Select(st => new { st.SkillTypeId, st.SkillTypeName }).ToListAsync(),
                    Categories = await _context.Categories.Select(c => new { c.CategoryId, c.CategoryName }).ToListAsync(),
                    Skills = await _context.Skills.Select(s => new { s.SkillId, s.SkillName, s.SkillTypeId }).ToListAsync(),
                    Professions = await _context.Professions.Select(p => new { p.ProfessionId, p.ProfessionName, p.CategoryId }).ToListAsync(),
                    UserProfileSkills = await _context.UserProfileSkills.Select(ups => new { ups.ProfileId, ups.SkillId }).ToListAsync(),
                    UserProfileProfessions = await _context.UserProfileProfessions.Select(upp => new { upp.ProfileId, upp.ProfessionId }).ToListAsync()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestSkillsProfessions(int profileId)
        {
            try
            {
                var profile = await _context.UserProfiles
                    .Include(p => p.UserProfileSkills)
                        .ThenInclude(ups => ups.Skill)
                    .Include(p => p.UserProfileProfessions)
                        .ThenInclude(upp => upp.Profession)
                    .FirstOrDefaultAsync(p => p.ProfileId == profileId);

                if (profile == null)
                {
                    return Json(new { error = "Profile not found" });
                }

                var skills = await _context.UserProfileSkills
                    .Where(ups => ups.ProfileId == profileId)
                    .Include(ups => ups.Skill)
                    .ToListAsync();

                var professions = await _context.UserProfileProfessions
                    .Where(upp => upp.ProfileId == profileId)
                    .Include(upp => upp.Profession)
                    .ToListAsync();

                return Json(new
                {
                    profileId = profileId,
                    skillsCount = skills.Count,
                    professionsCount = professions.Count,
                    skills = skills.Select(s => new { 
                        skillId = s.SkillId, 
                        skillName = s.Skill?.SkillName,
                        profileId = s.ProfileId
                    }),
                    professions = professions.Select(p => new { 
                        professionId = p.ProfessionId, 
                        professionName = p.Profession?.ProfessionName,
                        profileId = p.ProfileId
                    })
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

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
                    .FirstOrDefaultAsync(p => p.ProfileId == id && p.UserId == userId);

                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ hoặc bạn không có quyền chỉnh sửa.";
                    return RedirectToAction(nameof(Index));
                }

                // Load dropdown data
                try
                {
                    ViewBag.SkillTypes = await _context.SkillTypes
                        .Select(st => new { st.SkillTypeId, st.SkillTypeName })
                        .ToListAsync();
                    ViewBag.Categories = await _context.Categories
                        .Select(c => new { c.CategoryId, c.CategoryName })
                        .ToListAsync();
                    ViewBag.WorkTypes = await _context.WorkTypes
                        .Select(w => new { w.WorkTypeId, w.WorkTypeName })
                        .ToListAsync();
                    ViewBag.DegreeTypes = await _context.DegreeTypes
                        .Select(d => new { d.DegreeTypeId, d.DegreeTypeName })
                        .ToListAsync();
                    ViewBag.CertificateTypes = await _context.CertificateTypes
                        .Select(c => new { c.CertificateTypeId, c.CertificateTypeName })
                        .ToListAsync();
                }
                catch (Exception)
                {
                    ViewBag.SkillTypes = new List<object>();
                    ViewBag.Categories = new List<object>();
                    ViewBag.WorkTypes = new List<object>();
                    ViewBag.DegreeTypes = new List<object>();
                    ViewBag.CertificateTypes = new List<object>();
                    
                    TempData["Warning"] = "Một số dữ liệu dropdown không thể tải.";
                }

                return View(profile);
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải trang chỉnh sửa hồ sơ.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserProfile profile, string fullName, string phoneNumber, IFormFile avatar)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                var existingProfile = await _context.UserProfiles
                    .Include(p => p.User)
                    .Include(p => p.UserProfileSkills)
                    .Include(p => p.UserProfileProfessions)
                    .Include(p => p.WorkExperiences)
                    .Include(p => p.Degrees)
                    .Include(p => p.Certificates)
                    .FirstOrDefaultAsync(p => p.ProfileId == id && p.UserId == userId);

                if (existingProfile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ hoặc bạn không có quyền chỉnh sửa.";
                    return RedirectToAction(nameof(Index));
                }

                // Update user information
                if (existingProfile.User != null)
                {
                    existingProfile.User.FullName = fullName?.Trim() ?? existingProfile.User.FullName;
                    existingProfile.User.PhoneNumber = phoneNumber?.Trim() ?? existingProfile.User.PhoneNumber;

                    // Handle avatar upload
                    if (avatar != null && avatar.Length > 0)
                    {
                        try
                        {
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                            var fileExtension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
                            
                            if (allowedExtensions.Contains(fileExtension) && avatar.Length <= 5 * 1024 * 1024)
                            {
                                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                                Directory.CreateDirectory(uploadsFolder);
                                
                                var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(avatar.FileName)}";
                                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                                
                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await avatar.CopyToAsync(fileStream);
                                }
                                
                                existingProfile.User.Avata = $"/uploads/avatars/{uniqueFileName}";
                            }
                        }
                        catch (Exception)
                        {
                            TempData["Warning"] = "Không thể cập nhật ảnh đại diện.";
                        }
                    }

                    _context.Users.Update(existingProfile.User);
                }

                // Update profile information
                existingProfile.Summary = profile.Summary;
                existingProfile.Location = profile.Location;
                existingProfile.ExpectedSalary = profile.ExpectedSalary;
                existingProfile.UpdatedDate = DateTime.UtcNow;
                existingProfile.ApprovalStatus = "Pending"; // Reset approval status when edited

                // Clear existing skills and professions
                _context.UserProfileSkills.RemoveRange(existingProfile.UserProfileSkills);
                _context.UserProfileProfessions.RemoveRange(existingProfile.UserProfileProfessions);

                // Clear existing work experiences, degrees, certificates
                _context.WorkExperiences.RemoveRange(existingProfile.WorkExperiences);
                _context.Degrees.RemoveRange(existingProfile.Degrees);
                _context.Certificates.RemoveRange(existingProfile.Certificates);

                await _context.SaveChangesAsync();

                // Process new skills
                var skillNames = Request.Form["skillNames"].ToList();
                var skillTypeIds = Request.Form["skillTypeIds"].ToList();
                var skillsToAdd = new List<UserProfileSkill>();
                
                for (int i = 0; i < skillNames.Count && i < skillTypeIds.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(skillNames[i]) && int.TryParse(skillTypeIds[i], out var skillTypeId) && skillTypeId > 0)
                    {
                        var existingSkill = await _context.Skills
                            .FirstOrDefaultAsync(s => s.SkillName.ToLower() == skillNames[i].Trim().ToLower() && s.SkillTypeId == skillTypeId);
                        
                        if (existingSkill == null)
                        {
                            existingSkill = new Skill
                            {
                                SkillName = skillNames[i].Trim(),
                                SkillTypeId = skillTypeId
                            };
                            _context.Skills.Add(existingSkill);
                            await _context.SaveChangesAsync();
                        }
                        
                        skillsToAdd.Add(new UserProfileSkill
                        {
                            ProfileId = existingProfile.ProfileId,
                            SkillId = existingSkill.SkillId
                        });
                    }
                }

                // Process new professions
                var professionNames = Request.Form["professionNames"].ToList();
                var categoryIds = Request.Form["categoryIds"].ToList();
                var professionsToAdd = new List<UserProfileProfession>();
                
                for (int i = 0; i < professionNames.Count && i < categoryIds.Count; i++)
                {
                    if (!string.IsNullOrWhiteSpace(professionNames[i]) && int.TryParse(categoryIds[i], out var categoryId) && categoryId > 0)
                    {
                        var existingProfession = await _context.Professions
                            .FirstOrDefaultAsync(p => p.ProfessionName.ToLower() == professionNames[i].Trim().ToLower() && p.CategoryId == categoryId);
                        
                        if (existingProfession == null)
                        {
                            existingProfession = new Profession
                            {
                                ProfessionName = professionNames[i].Trim(),
                                CategoryId = categoryId
                            };
                            _context.Professions.Add(existingProfession);
                            await _context.SaveChangesAsync();
                        }
                        
                        professionsToAdd.Add(new UserProfileProfession
                        {
                            ProfileId = existingProfile.ProfileId,
                            ProfessionId = existingProfession.ProfessionId
                        });
                    }
                }

                // Process work experiences, degrees, and certificates
                await ProcessWorkExperiences(existingProfile.ProfileId);
                await ProcessDegrees(existingProfile.ProfileId, userId);
                await ProcessCertificates(existingProfile.ProfileId, userId);

                // Add new skills and professions
                if (skillsToAdd.Any())
                {
                    _context.UserProfileSkills.AddRange(skillsToAdd);
                }
                
                if (professionsToAdd.Any())
                {
                    _context.UserProfileProfessions.AddRange(professionsToAdd);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Hồ sơ đã được cập nhật thành công! Hồ sơ đang chờ admin duyệt lại.";
                return RedirectToAction(nameof(Details), new { id = existingProfile.ProfileId });
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi cập nhật hồ sơ. Vui lòng thử lại.";
                return RedirectToAction(nameof(Edit), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                // First, find the profile
                var profile = await _context.UserProfiles
                    .FirstOrDefaultAsync(p => p.ProfileId == id && p.UserId == userId);

                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ hoặc bạn không có quyền xóa.";
                    return RedirectToAction(nameof(Index));
                }

                // Delete related data step by step using raw SQL with error handling
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM UserProfileSkills WHERE UserProfileId = {0}", id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting UserProfileSkills: {ex.Message}");
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM UserProfileProfessions WHERE UserProfileId = {0}", id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting UserProfileProfessions: {ex.Message}");
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM WorkExperiences WHERE UserProfileId = {0}", id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting WorkExperiences: {ex.Message}");
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Degrees WHERE UserProfileId = {0}", id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting Degrees: {ex.Message}");
                }

                try
                {
                    await _context.Database.ExecuteSqlRawAsync("DELETE FROM Certificates WHERE UserProfileId = {0}", id);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting Certificates: {ex.Message}");
                }
                
                // Finally delete the profile
                await _context.Database.ExecuteSqlRawAsync("DELETE FROM UserProfiles WHERE ProfileId = {0}", id);

                TempData["Success"] = "Hồ sơ đã được xóa thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting profile: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                
                TempData["Error"] = $"Có lỗi xảy ra khi xóa hồ sơ: {ex.Message}. Vui lòng thử lại.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<IActionResult> DebugDelete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                var profile = await _context.UserProfiles
                    .Include(p => p.UserProfileSkills)
                    .Include(p => p.UserProfileProfessions)
                    .Include(p => p.WorkExperiences)
                    .Include(p => p.Degrees)
                    .Include(p => p.Certificates)
                    .Include(p => p.CVs)
                    .FirstOrDefaultAsync(p => p.ProfileId == id && p.UserId == userId);

                if (profile == null)
                {
                    return Json(new { error = "Profile not found" });
                }

                var debugInfo = new
                {
                    profileId = profile.ProfileId,
                    userId = profile.UserId,
                    skillsCount = profile.UserProfileSkills.Count,
                    professionsCount = profile.UserProfileProfessions.Count,
                    workExperiencesCount = profile.WorkExperiences.Count,
                    degreesCount = profile.Degrees.Count,
                    certificatesCount = profile.Certificates.Count,
                    cvsCount = profile.CVs.Count,
                    skills = profile.UserProfileSkills.Select(s => new { s.SkillId, s.ProfileId }),
                    professions = profile.UserProfileProfessions.Select(p => new { p.ProfessionId, p.ProfileId }),
                    workExperiences = profile.WorkExperiences.Select(w => new { w.ExperienceId, w.ProfileId }),
                    degrees = profile.Degrees.Select(d => new { d.DegreeId, d.ProfileId }),
                    certificates = profile.Certificates.Select(c => new { c.CertificateId, c.ProfileId }),
                    cvs = profile.CVs.Select(cv => new { cv.CVId, cv.ProfileId })
                };

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, innerException = ex.InnerException?.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CVView(int? id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                UserProfile? profile;
                
                if (id.HasValue)
                {
                    profile = await _context.UserProfiles
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
                        .FirstOrDefaultAsync(p => p.ProfileId == id.Value && p.UserId == userId);
                }
                else
                {
                    profile = await _context.UserProfiles
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
                        .Where(p => p.UserId == userId)
                        .OrderByDescending(p => p.CreatedDate)
                        .FirstOrDefaultAsync();
                }

                if (profile == null)
                {
                    TempData["Error"] = "Không tìm thấy hồ sơ. Vui lòng tạo hồ sơ trước.";
                    return RedirectToAction(nameof(Create));
                }

                return View(profile);
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải CV. Vui lòng thử lại sau.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Settings actions for user profile management
        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Login", "Account");
                }

                // Get current user information
                var currentUser = await _context.Users.FindAsync(userId);
                if (currentUser == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Index", "Home");
                }

                return View(currentUser);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải trang cài đặt. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber, IFormFile avatar)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng.";
                    return RedirectToAction("Settings");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Settings");
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(fullName))
                {
                    TempData["Error"] = "Họ và tên không được để trống!";
                    return RedirectToAction("Settings");
                }

                // Update basic info
                user.FullName = fullName.Trim();
                user.PhoneNumber = phoneNumber?.Trim();

                // Handle avatar upload
                if (avatar != null && avatar.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(avatar.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        TempData["Error"] = "Chỉ chấp nhận file ảnh (.jpg, .jpeg, .png, .gif)!";
                        return RedirectToAction("Settings");
                    }

                    // Validate file size (5MB max)
                    if (avatar.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "Kích thước file không được vượt quá 5MB!";
                        return RedirectToAction("Settings");
                    }

                    // Create uploads directory if it doesn't exist
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "avatars");
                    if (!Directory.Exists(uploadsDir))
                    {
                        Directory.CreateDirectory(uploadsDir);
                    }

                    // Generate unique filename
                    var fileName = $"{userId}_{DateTime.Now.Ticks}{fileExtension}";
                    var filePath = Path.Combine(uploadsDir, fileName);

                    // Delete old avatar if exists
                    if (!string.IsNullOrEmpty(user.Avata))
                    {
                        var oldAvatarPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Avata.TrimStart('/'));
                        if (System.IO.File.Exists(oldAvatarPath))
                        {
                            System.IO.File.Delete(oldAvatarPath);
                        }
                    }

                    // Save new avatar
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(stream);
                    }

                    user.Avata = $"/uploads/avatars/{fileName}";
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Thông tin cá nhân đã được cập nhật thành công!";
                
                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi cập nhật thông tin: {ex.Message}";
                return RedirectToAction("Settings");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                
                if (userId == 0)
                {
                    TempData["Error"] = "Không thể xác định thông tin người dùng.";
                    return RedirectToAction("Settings");
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    TempData["Error"] = "Không tìm thấy thông tin người dùng!";
                    return RedirectToAction("Settings");
                }

                // Validate input
                if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ thông tin mật khẩu!";
                    return RedirectToAction("Settings");
                }

                if (newPassword != confirmPassword)
                {
                    TempData["Error"] = "Mật khẩu mới và xác nhận mật khẩu không khớp!";
                    return RedirectToAction("Settings");
                }

                if (newPassword.Length < 8)
                {
                    TempData["Error"] = "Mật khẩu mới phải có ít nhất 8 ký tự!";
                    return RedirectToAction("Settings");
                }

                // Verify current password
                if (!PasswordUtils.Verify(currentPassword, user.Password))
                {
                    TempData["Error"] = "Mật khẩu hiện tại không đúng!";
                    return RedirectToAction("Settings");
                }

                // Update password
                user.Password = PasswordUtils.Hash(newPassword);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Mật khẩu đã được thay đổi thành công!";
                return RedirectToAction("Settings");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Có lỗi xảy ra khi thay đổi mật khẩu: {ex.Message}";
                return RedirectToAction("Settings");
            }
        }
    }
}