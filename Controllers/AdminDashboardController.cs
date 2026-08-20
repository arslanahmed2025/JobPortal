using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using JobPortal.Data;
using JobPortal.Models;
using JobPortal.Models.ViewModels;

namespace JobPortal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminDashboardController(ApplicationDbContext context,
                                         UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: /AdminDashboard
        public async Task<IActionResult> Index()
        {
            ViewBag.TotalUsers = await _context.Users.CountAsync();
            ViewBag.TotalEmployers = (await _userManager.GetUsersInRoleAsync("Employer")).Count;
            ViewBag.TotalJobSeekers = (await _userManager.GetUsersInRoleAsync("JobSeeker")).Count;
            ViewBag.TotalJobs = await _context.JobPostings.CountAsync();
            ViewBag.TotalApplications = await _context.JobApplications.CountAsync();

            return View();
        }

        // GET: /AdminDashboard/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<AdminUserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var isBlocked = await _userManager.IsLockedOutAsync(user);

                model.Add(new AdminUserViewModel
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "N/A",
                    IsBlocked = isBlocked
                });
            }

            return View(model);
        }

        // POST: /AdminDashboard/ToggleUserStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                TempData["ErrorMessage"] = "🚫You cannot block the admin account 👑🔒.";
                return RedirectToAction("Users");
            }

            var isBlocked = await _userManager.IsLockedOutAsync(user);

            if (isBlocked)
            {
                // Unblock
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["SuccessMessage"] = $"{user.Email} unblock ho gaya.";
            }
            else
            {
                // Block
                if (!user.LockoutEnabled)
                {
                    await _userManager.SetLockoutEnabledAsync(user, true);
                }
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                TempData["SuccessMessage"] = $"{user.Email} block ho gaya.";
            }

            return RedirectToAction("Users");
        }

        // GET: /AdminDashboard/Jobs
        public async Task<IActionResult> Jobs()
        {
            var jobs = await _context.JobPostings
                .Include(j => j.EmployerProfile)
                .OrderByDescending(j => j.PostedDate)
                .ToListAsync();

            return View(jobs);
        }

        // POST: /AdminDashboard/DeleteJob/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _context.JobPostings.FindAsync(id);
            if (job != null)
            {
                _context.JobPostings.Remove(job);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The job has been deleted.";
            }

            return RedirectToAction("Jobs");
        }
    }
}