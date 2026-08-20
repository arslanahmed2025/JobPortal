using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortal.Models
{
    public class EmployerProfile
    {
        [Key]
        public int Id { get; set; }

        // Link to Identity user (Employer role wala)
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        [StringLength(150)]
        public string CompanyName { get; set; }

        [StringLength(1000)]
        public string? CompanyDescription { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        public int? FoundedYear { get; set; }

        [StringLength(50)]
        public string? CompanySize { get; set; } // e.g. "1-10", "11-50", "51-200"

        public string? LogoPath { get; set; } // company logo image path

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public ICollection<JobPosting>? JobPostings { get; set; }
    }
}