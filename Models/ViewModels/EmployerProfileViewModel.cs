using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models.ViewModels
{
    public class EmployerProfileViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name zaroori hai")]
        [StringLength(150)]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [StringLength(1000)]
        [Display(Name = "Company Description")]
        public string? CompanyDescription { get; set; }

        [StringLength(200)]
        public string? Website { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [StringLength(100)]
        public string? Location { get; set; }

        [Display(Name = "Founded Year")]
        public int? FoundedYear { get; set; }

        [StringLength(50)]
        [Display(Name = "Company Size")]
        public string? CompanySize { get; set; }

        // Existing logo path (edit ke waqt dikhane ke liye)
        public string? ExistingLogoPath { get; set; }

        // New uploaded file
        [Display(Name = "Company Logo")]
        public IFormFile? LogoFile { get; set; }
    }
}