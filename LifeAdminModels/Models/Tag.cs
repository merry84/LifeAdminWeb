using System.ComponentModel.DataAnnotations;

namespace LifeAdminModels.Models
{
    public class Tag
    {
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public ICollection<TaskItemTag> TaskItems { get; set; } = new HashSet<TaskItemTag>();
    }
}