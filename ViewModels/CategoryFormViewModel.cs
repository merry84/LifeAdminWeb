using System.ComponentModel.DataAnnotations;
using static LifeAdminModels.GCommons.DataConstants.CategoryFormViewModel;


namespace ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;
    }
}
