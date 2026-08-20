using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal.Data;
using JobPortal.Models;
using JobPortal.Models.ViewModels;

namespace JobPortal.Controllers
{
    [Authorize(Roles = "JobSeeker")]
    public class JobSeekerDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public JobSeekerDashboardController(ApplicationDbContext context,
                                             UserManager<ApplicationUser> userManager,
                                             IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /JobSeekerDashboard — active jobs ki list (browse)
        public async Task<IActionResult> Index(string? search)
        {
            var query = _context.JobPostings
                .Include(j => j.EmployerProfile)
                .Where(j => j.IsActive)
                .OrderByDescending(j => j.PostedDate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(j => j.Title.Contains(search)
                                       || (j.Category != null && j.Category.Contains(search))
                                       || (j.Location != null && j.Location.Contains(search)));
            }

            var jobs = await query.ToListAsync();
            ViewBag.Search = search;

            return View(jobs);
        }

        // GET: /JobSeekerDashboard/JobDetails/5
        public async Task<IActionResult> JobDetails(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.EmployerProfile)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobPostingId == id && a.JobSeekerUserId == userId);

            ViewBag.AlreadyApplied = alreadyApplied;

            return View(job);
        }

        // GET: /JobSeekerDashboard/Apply/5
        public async Task<IActionResult> Apply(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.EmployerProfile)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            var alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobPostingId == id && a.JobSeekerUserId == userId);

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] = "Aap is job pe pehle hi apply kar chuke hain.";
                return RedirectToAction("JobDetails", new { id });
            }

            var model = new JobApplyViewModel
            {
                JobPostingId = job.Id,
                JobTitle = job.Title,
                CompanyName = job.EmployerProfile.CompanyName
            };

            return View(model);
        }

        // POST: /JobSeekerDashboard/Apply
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(JobApplyViewModel model)
        {
            var userId = _userManager.GetUserId(User);

            var alreadyApplied = await _context.JobApplications
                .AnyAsync(a => a.JobPostingId == model.JobPostingId && a.JobSeekerUserId == userId);

            if (alreadyApplied)
            {
                TempData["ErrorMessage"] = "Aap is job pe pehle hi apply kar chuke hain.";
                return RedirectToAction("JobDetails", new { id = model.JobPostingId });
            }

            if (!ModelState.IsValid)
            {
                var job = await _context.JobPostings
                    .Include(j => j.EmployerProfile)
                    .FirstOrDefaultAsync(j => j.Id == model.JobPostingId);

                model.JobTitle = job?.Title;
                model.CompanyName = job?.EmployerProfile?.CompanyName;
                return View(model);
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "resumes");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(model.ResumeFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.ResumeFile.CopyToAsync(stream);
            }

            var application = new JobApplication
            {
                JobPostingId = model.JobPostingId,
                JobSeekerUserId = userId,
                CoverLetter = model.CoverLetter,
                ResumePath = "/uploads/resumes/" + uniqueFileName,
                Status = ApplicationStatus.Pending,
                AppliedDate = DateTime.Now
            };

            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Application submit ho gayi!";
            return RedirectToAction("MyApplications");
        }

        // GET: /JobSeekerDashboard/MyApplications
        public async Task<IActionResult> MyApplications()
        {
            var userId = _userManager.GetUserId(User);

            var applications = await _context.JobApplications
                .Include(a => a.JobPosting)
                    .ThenInclude(j => j.EmployerProfile)
                .Where(a => a.JobSeekerUserId == userId)
                .OrderByDescending(a => a.AppliedDate)
                .ToListAsync();

            return View(applications);
        }
    }
}