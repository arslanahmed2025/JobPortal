using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using JobPortal.Models;

namespace JobPortal.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmployerProfile> EmployerProfiles { get; set; }
        public DbSet<JobPosting> JobPostings { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<JobPosting>()
                .Property(j => j.SalaryMin)
                .HasColumnType("decimal(18,2)");

            builder.Entity<JobPosting>()
                .Property(j => j.SalaryMax)
                .HasColumnType("decimal(18,2)");

            builder.Entity<EmployerProfile>()
                .HasIndex(e => e.UserId)
                .IsUnique();

            builder.Entity<JobPosting>()
                .HasOne(j => j.EmployerProfile)
                .WithMany(e => e.JobPostings)
                .HasForeignKey(j => j.EmployerProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // JobApplication relationships
            builder.Entity<JobApplication>()
                .HasOne(a => a.JobPosting)
                .WithMany()
                .HasForeignKey(a => a.JobPostingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<JobApplication>()
                .HasOne(a => a.JobSeeker)
                .WithMany()
                .HasForeignKey(a => a.JobSeekerUserId)
                .OnDelete(DeleteBehavior.Restrict); // multiple cascade path error se bachne ke liye

            // Ek JobSeeker ek job pe sirf ek baar apply kar sake
            builder.Entity<JobApplication>()
                .HasIndex(a => new { a.JobPostingId, a.JobSeekerUserId })
                .IsUnique();
        }
    }
}