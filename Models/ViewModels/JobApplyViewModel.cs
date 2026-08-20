using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models.ViewModels
{
    public class JobApplyViewModel
    {
        public int JobPostingId { get; set; }

        public string? JobTitle { get; set; } // display ke liye
        public string? CompanyName { get; set; } // display ke liye

        [StringLength(1000)]
        [Display(Name = "Cover Letter (optional)")]
        public string? CoverLetter { get; set; }

        [Required(ErrorMessage = "Resume upload karna zaroori hai")]
        [Display(Name = "Resume (PDF)")]
        public IFormFile ResumeFile { get; set; }
    }
}