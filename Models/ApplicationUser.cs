using Microsoft.AspNetCore.Identity;

namespace JobPortal.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true; // admin block/unblock ke liye
    }
}