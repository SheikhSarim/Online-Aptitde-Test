using OnlineAptitdeTest.Models;
using OnlineAptitdeTest.Models.OnlineAptitdeTest.Models;
using OnlineAptitdeTest.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Data;

namespace OnlineAptitdeTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Candidate> Candidates { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Admin" }
            );

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Email = "admin@gmail.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123")
                }
            );

            
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Admins)
                .WithMany(a => a.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Managers)
                .WithMany(m => m.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Candidate)
                .WithMany(c => c.Results)  
                .HasForeignKey(r => r.CandidateId);

            modelBuilder.Entity<Result>()
                .HasOne(r => r.Test)
                .WithMany(t => t.Results)
                .HasForeignKey(r => r.TestId);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Test)
                .WithMany(t => t.Questions) 
                .HasForeignKey(q => q.TestId);
        }

        public DbSet<OnlineAptitdeTest.Models.ViewModels.LoginViewModel> LoginViewModel { get; set; } = default!;

        public DbSet<OnlineAptitdeTest.Models.ViewModels.RegisterViewModel> RegisterViewModel { get; set; } = default!;

        public DbSet<OnlineAptitdeTest.Models.ViewModels.CreateCandidateViewModel> CreateCandidateViewModel { get; set; } = default!;

        public DbSet<OnlineAptitdeTest.Models.ViewModels.EditCandidateViewModel> EditCandidateViewModel { get; set; } = default!;

        public DbSet<OnlineAptitdeTest.Models.ViewModels.CandidateLoginViewModel> CandidateLoginViewModel { get; set; } = default!;

        public DbSet<OnlineAptitdeTest.Models.ViewModels.TestViewModel> TestViewModel { get; set; } = default!;
        public DbSet<OnlineAptitdeTest.Models.ViewModels.QuestionViewModel> QuestionViewModel { get; set; } = default!;
        public DbSet<OnlineAptitdeTest.Models.ViewModels.ResultViewModel> ResultViewModel { get; set; } = default!;
        public DbSet<OnlineAptitdeTest.Models.ViewModels.CandidateResultViewModel> CandidateResultViewModel { get; set; } = default!;

    }

}
