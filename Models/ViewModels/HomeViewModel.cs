using JobPortal.Models;

namespace JobPortal.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<JobPosting> RecentJobs { get; set; } = new();
        public List<JobPosting> DeadlineSoonJobs { get; set; } = new();
        public int SelectedThisMonth { get; set; }
        public int TotalActiveJobs { get; set; }
        public int TotalEmployers { get; set; }

        // Employee (Employer) of the Month
        public EmployerProfile? FeaturedEmployer { get; set; }
        public int FeaturedEmployerJobCount { get; set; }
    }
}