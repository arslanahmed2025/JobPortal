using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortal.Models
{
    public enum ApplicationStatus
    {
        Pending,
        Reviewed,
        Shortlisted,
        Rejected,
        Accepted
    }

    public class JobApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int JobPostingId { get; set; }

        [ForeignKey("JobPostingId")]
        public JobPosting JobPosting { get; set; }

        [Required]
        public string JobSeekerUserId { get; set; }

        [ForeignKey("JobSeekerUserId")]
        public ApplicationUser JobSeeker { get; set; }

        [StringLength(1000)]
        public string? CoverLetter { get; set; }

        public string? ResumePath { get; set; }

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime AppliedDate { get; set; } = DateTime.Now;
    }
}