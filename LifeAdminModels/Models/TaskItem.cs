using System.ComponentModel.DataAnnotations;


namespace LifeAdminModels.Models
{
    public class TaskItem
    {
        public int Id { get; set; }

        [Required, MaxLength(80)]
        public string Title { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public WorkStatus Status { get; set; }


        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;
    }
}

