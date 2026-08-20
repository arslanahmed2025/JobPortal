using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal.Data;
using JobPortal.Models;
using JobPortal.Models.ViewModels;

namespace JobPortal.Controllers
{
    [Authorize(Roles = "Employer")]
    public class EmployerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployerDashboardController(ApplicationDbContext context,
                                            UserManager<ApplicationUser> userManager,
                                            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // Helper: current employer ka profile nikalna
        private async Task<EmployerProfile?> GetCurrentEmployerProfile()
        {
            var userId = _userManager.GetUserId(User);
            return await _context.EmployerProfiles
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // GET: /EmployerDashboard
        public async Task<IActionResult> Index()
        {
            var profile = await GetCurrentEmployerProfile();

            if (profile != null)
            {
                var jobs = await _context.JobPostings
                    .Where(j => j.EmployerProfileId == profile.Id)
                    .OrderByDescending(j => j.PostedDate)
                    .ToListAsync();

                ViewBag.JobPostings = jobs;
            }

            return View(profile);
        }

        // ================= PROFILE ACTIONS =================

        // GET: /EmployerDashboard/Profile
        public async Task<IActionResult> Profile()
        {
            var profile = await GetCurrentEmployerProfile();
            var model = new EmployerProfileViewModel();

            if (profile != null)
            {
                model.Id = profile.Id;
                model.CompanyName = profile.CompanyName;
                model.CompanyDescription = profile.CompanyDescription;
                model.Website = profile.Website;
                model.Industry = profile.Industry;
                model.Location = profile.Location;
                model.FoundedYear = profile.FoundedYear;
                model.CompanySize = profile.CompanySize;
                model.ExistingLogoPath = profile.LogoPath;
            }

            return View(model);
        }

        // POST: /EmployerDashboard/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(EmployerProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            var profile = await GetCurrentEmployerProfile();
            bool isNew = profile == null;

            if (isNew)
            {
                profile = new EmployerProfile { UserId = userId };
            }

            profile.CompanyName = model.CompanyName;
            profile.CompanyDescription = model.CompanyDescription;
            profile.Website = model.Website;
            profile.Industry = model.Industry;
            profile.Location = model.Location;
            profile.FoundedYear = model.FoundedYear;
            profile.CompanySize = model.CompanySize;

            if (model.LogoFile != null && model.LogoFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "logos");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                if (!isNew && !string.IsNullOrEmpty(profile.LogoPath))
                {
                    var oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, profile.LogoPath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.LogoFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.LogoFile.CopyToAsync(stream);
                }

                profile.LogoPath = "/uploads/logos/" + uniqueFileName;
            }

            if (isNew)
            {
                profile.CreatedAt = DateTime.Now;
                _context.EmployerProfiles.Add(profile);
            }
            else
            {
                _context.EmployerProfiles.Update(profile);
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Company profile save ho gaya!";
            return RedirectToAction("Index");
        }

        // ================= JOB POSTING ACTIONS =================

        // GET: /EmployerDashboard/JobCreate
        public async Task<IActionResult> JobCreate()
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Pehle company profile complete karo.";
                return RedirectToAction("Profile");
            }

            return View(new JobPostingViewModel());
        }

        // POST: /EmployerDashboard/JobCreate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobCreate(JobPostingViewModel model)
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null)
            {
                TempData["ErrorMessage"] = "Pehle company profile complete karo.";
                return RedirectToAction("Profile");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var job = new JobPosting
            {
                EmployerProfileId = profile.Id,
                Title = model.Title,
                Description = model.Description,
                Requirements = model.Requirements,
                Location = model.Location,
                JobType = model.JobType,
                Category = model.Category,
                SalaryMin = model.SalaryMin,
                SalaryMax = model.SalaryMax,
                Deadline = model.Deadline,
                IsActive = model.IsActive,
                PostedDate = DateTime.Now
            };

            _context.JobPostings.Add(job);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job posting create ho gayi!";
            return RedirectToAction("Index");
        }

        // GET: /EmployerDashboard/JobEdit/5
        public async Task<IActionResult> JobEdit(int id)
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null) return RedirectToAction("Profile");

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerProfileId == profile.Id);

            if (job == null)
            {
                return NotFound();
            }

            var model = new JobPostingViewModel
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                Location = job.Location,
                JobType = job.JobType,
                Category = job.Category,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                Deadline = job.Deadline,
                IsActive = job.IsActive
            };

            return View(model);
        }

        // POST: /EmployerDashboard/JobEdit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobEdit(int id, JobPostingViewModel model)
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null) return RedirectToAction("Profile");

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerProfileId == profile.Id);

            if (job == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.Id = id;
                return View(model);
            }

            job.Title = model.Title;
            job.Description = model.Description;
            job.Requirements = model.Requirements;
            job.Location = model.Location;
            job.JobType = model.JobType;
            job.Category = model.Category;
            job.SalaryMin = model.SalaryMin;
            job.SalaryMax = model.SalaryMax;
            job.Deadline = model.Deadline;
            job.IsActive = model.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Job posting update ho gayi!";
            return RedirectToAction("Index");
        }

        // POST: /EmployerDashboard/JobDelete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> JobDelete(int id)
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null) return RedirectToAction("Profile");

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerProfileId == profile.Id);

            if (job != null)
            {
                _context.JobPostings.Remove(job);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Job posting delete ho gayi!";
            }

            return RedirectToAction("Index");
        }

        // ================= JOB APPLICATIONS (naya) =================

        // GET: /EmployerDashboard/JobApplications/5  (jobId)
        public async Task<IActionResult> JobApplications(int id)
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null) return RedirectToAction("Profile");

            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.Id == id && j.EmployerProfileId == profile.Id);

            if (job == null)
            {
                return NotFound();
            }

            var applications = await _context.JobApplications
                .Include(a => a.JobSeeker)
                .Where(a => a.JobPostingId == id)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            ViewBag.JobTitle = job.Title;
            ViewBag.JobId = job.Id;

            return View(applications);
        }

        // POST: /EmployerDashboard/UpdateApplicationStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateApplicationStatus(int applicationId, ApplicationStatus status, int jobId)
        {
            var profile = await GetCurrentEmployerProfile();
            if (profile == null) return RedirectToAction("Profile");

            var application = await _context.JobApplications
                .Include(a => a.JobPosting)
                .FirstOrDefaultAsync(a => a.Id == applicationId
                                       && a.JobPosting.EmployerProfileId == profile.Id);

            if (application != null)
            {
                application.Status = status;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Application status update ho gaya!";
            }

            return RedirectToAction("JobApplications", new { id = jobId });
        }
    }
}