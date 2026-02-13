using LifeAdminModels.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace LifeAdminData
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Owner)
                .WithMany()
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Note>()
                .HasOne(n => n.Owner)
                .WithMany()
                .HasForeignKey(n => n.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Category>()
                .HasData(
                 new Category { Id = 1, Name = "Work" },
                 new Category { Id = 2, Name = "Personal" },
                 new Category { Id = 3, Name = "Health" }
            );
        }
    }
}
