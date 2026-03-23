using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.Category;

namespace LifeAdminModels.Models
{
    public class Category
    {
        public Guid Id { get; set; }

        [Required, MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;

        public ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();
    }
}
