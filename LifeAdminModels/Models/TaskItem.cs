using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.TaskItem;


namespace LifeAdminModels.Models
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        [Required, MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public WorkStatus Status { get; set; }


        public Guid CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        [Required]
        
        public required string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; } = null!;

        public ICollection<TaskItemTag> TaskItemTags { get; set; }
            = new HashSet<TaskItemTag>();
    }
}

