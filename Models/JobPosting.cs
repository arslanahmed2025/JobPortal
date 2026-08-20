using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobPortal.Models
{
    public enum JobType
    {
        FullTime,
        PartTime,
        Contract,
        Internship,
        Remote
    }

    public class JobPosting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EmployerProfileId { get; set; }

        [ForeignKey("EmployerProfileId")]
        public EmployerProfile EmployerProfile { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(1000)]
        public string? Requirements { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Required]
        public JobType JobType { get; set; }

        [StringLength(100)]
        public string? Category { get; set; } // e.g. "IT", "Marketing", "Sales"

        public decimal? SalaryMin { get; set; }

        public decimal? SalaryMax { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.Now;

        public DateTime? Deadline { get; set; }

        public bool IsActive { get; set; } = true;
    }
}