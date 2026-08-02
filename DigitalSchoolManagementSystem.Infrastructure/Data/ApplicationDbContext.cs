using DigitalSchoolManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalSchoolManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<StaffUser> StaffUsers => Set<StaffUser>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<Subject> Subjects => Set<Subject>();
        public DbSet<Attendance> Attendances => Set<Attendance>();
        public DbSet<Exam> Exams => Set<Exam>();
        public DbSet<ExamSubject> ExamSubjects => Set<ExamSubject>();
        public DbSet<ExamResult> ExamResults => Set<ExamResult>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(250);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);

                entity.HasOne(e => e.Role)
                      .WithMany(r => r.Users)
                      .HasForeignKey(e => e.RoleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasIndex(e => e.AdmissionNumber).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.Property(e => e.AdmissionNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RollNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Grade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Section).IsRequired().HasMaxLength(20);

                entity.HasOne(e => e.User)
                      .WithOne(u => u.Student)
                      .HasForeignKey<Student>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<StaffUser>(entity =>
            {
                entity.HasIndex(e => e.EmployeeCode).IsUnique();
                entity.HasIndex(e => e.UserId).IsUnique();

                entity.Property(e => e.EmployeeCode).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Designation).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.Qualification).HasMaxLength(150);
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.User)
                      .WithOne(u => u.StaffUser)
                      .HasForeignKey<StaffUser>(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasIndex(e => e.Token).IsUnique();

                entity.Property(e => e.Token).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ReplacedByToken).HasMaxLength(200);

                entity.HasOne(e => e.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(e => e.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasIndex(e => new { e.Grade, e.Code }).IsUnique();

                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Grade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Description).HasMaxLength(250);
            });

            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.Date }).IsUnique();

                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.HasOne(e => e.Student)
                      .WithMany()
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Grade).IsRequired().HasMaxLength(50);
                entity.Property(e => e.AcademicYear).IsRequired().HasMaxLength(20);
            });

            modelBuilder.Entity<ExamSubject>(entity =>
            {
                entity.HasIndex(e => new { e.ExamId, e.SubjectId }).IsUnique();

                entity.Property(e => e.MaxMarks).HasColumnType("decimal(5,2)");
                entity.Property(e => e.PassingMarks).HasColumnType("decimal(5,2)");

                entity.HasOne(e => e.Exam)
                      .WithMany(e => e.ExamSubjects)
                      .HasForeignKey(e => e.ExamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Subject)
                      .WithMany()
                      .HasForeignKey(e => e.SubjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ExamResult>(entity =>
            {
                entity.HasIndex(e => new { e.StudentId, e.ExamSubjectId }).IsUnique();

                entity.Property(e => e.MarksObtained).HasColumnType("decimal(5,2)");
                entity.Property(e => e.Grade).HasMaxLength(5);
                entity.Property(e => e.Remarks).HasMaxLength(250);

                entity.HasOne(e => e.Student)
                      .WithMany()
                      .HasForeignKey(e => e.StudentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ExamSubject)
                      .WithMany()
                      .HasForeignKey(e => e.ExamSubjectId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
