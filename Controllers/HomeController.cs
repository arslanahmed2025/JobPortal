using System.Diagnostics;
using JobPortal.Data;
using JobPortal.Models;
using JobPortal.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
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

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var model = new HomeViewModel();

            // 1. Recent job notifications/updates — latest 5 active jobs
            model.RecentJobs = await _context.JobPostings
                .Include(j => j.EmployerProfile)
                .Where(j => j.IsActive)
                .OrderByDescending(j => j.PostedDate)
                .Take(5)
                .ToListAsync();

            // 2. Future opportunities — deadline ke qareeb (agle 14 din, abhi tak expire nahi hui)
            model.DeadlineSoonJobs = await _context.JobPostings
                .Include(j => j.EmployerProfile)
                .Where(j => j.IsActive
                         && j.Deadline != null
                         && j.Deadline >= now
                         && j.Deadline <= now.AddDays(14))
                .OrderBy(j => j.Deadline)
                .Take(5)
                .ToListAsync();

            // 3. Iss month kitne JobSeeker select (Accepted) huye
            model.SelectedThisMonth = await _context.JobApplications
                .CountAsync(a => a.Status == ApplicationStatus.Accepted
                               && a.AppliedDate >= monthStart);

            // Extra stats
            model.TotalActiveJobs = await _context.JobPostings.CountAsync(j => j.IsActive);
            model.TotalEmployers = await _context.EmployerProfiles.CountAsync();

            // 4. Employee (Employer) of the Month — sabse zyada jobs post karne wala
            var topEmployer = await _context.JobPostings
                .GroupBy(j => j.EmployerProfileId)
                .Select(g => new { EmployerProfileId = g.Key, JobCount = g.Count() })
                .OrderByDescending(g => g.JobCount)
                .FirstOrDefaultAsync();

            if (topEmployer != null)
            {
                model.FeaturedEmployer = await _context.EmployerProfiles
                    .FirstOrDefaultAsync(e => e.Id == topEmployer.EmployerProfileId);
                model.FeaturedEmployerJobCount = topEmployer.JobCount;
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}