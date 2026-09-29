using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Models;

namespace StudentResultManagement.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Mark> Marks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Prevent duplicate register number
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.RegisterNumber)
                .IsUnique();

            // Prevent duplicate subject code
            modelBuilder.Entity<Subject>()
                .HasIndex(s => s.SubjectCode)
                .IsUnique();

            // Prevent duplicate student + subject marks
            modelBuilder.Entity<Mark>()
                .HasIndex(m => new { m.StudentId, m.SubjectId })
                .IsUnique();
        }
    }
}
