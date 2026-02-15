using System.ComponentModel.DataAnnotations;
using static LifeAdminModels.GCommons.DataConstants.Category;

namespace LifeAdminModels.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required, MaxLength(NameMaxLength)]
        public string Name { get; set; } = null!;
    }
}
