using System.ComponentModel.DataAnnotations;
using JobPortal.Models;

namespace JobPortal.Models.ViewModels
{
    public class JobPostingViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Job title zaroori hai")]
        [StringLength(150)]
        [Display(Name = "Job Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description zaroori hai")]
        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string? Requirements { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Required]
        [Display(Name = "Job Type")]
        public JobType JobType { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [Display(Name = "Minimum Salary")]
        public decimal? SalaryMin { get; set; }

        [Display(Name = "Maximum Salary")]
        public decimal? SalaryMax { get; set; }

        [Display(Name = "Application Deadline")]
        [DataType(DataType.Date)]
        public DateTime? Deadline { get; set; }

        public bool IsActive { get; set; } = true;
    }
}