using LifeAdminModels.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LifeAdminData
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Note> Notes { get; set; } = null!;

        public DbSet<Document> Documents { get; set; } = null!;

        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<TaskItemTag> TaskItemTags { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Owner)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<TaskItem>()
               .HasOne(t => t.Owner)
               .WithMany(u => u.Tasks)
               .HasForeignKey(t => t.OwnerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Tasks)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TaskItem>()
                .HasQueryFilter(t => !t.IsDeleted);

            builder.Entity<Note>()
                .HasOne(n => n.Owner)
                .WithMany(u => u.Notes)
                .HasForeignKey(n => n.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Note>()
                .HasQueryFilter(n => !n.IsDeleted);

            builder.Entity<Document>()
                .HasOne(d => d.Owner)
                .WithMany(u => u.Documents)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Document>()
                .HasQueryFilter(d => !d.IsDeleted);

            builder.Entity<TaskItemTag>()
                .HasKey(tt => new { tt.TaskItemId, tt.TagId });

            builder.Entity<TaskItemTag>()
                .HasOne(tt => tt.TaskItem)
                .WithMany(t => t.TaskItemTags)
                .HasForeignKey(tt => tt.TaskItemId);

            builder.Entity<TaskItemTag>()
                .HasOne(tt => tt.Tag)
                .WithMany(t => t.TaskItems)
                .HasForeignKey(tt => tt.TagId);

           
        }
    }
}