using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.CategoryFormViewModel;

namespace ViewModels.CategoriesViewModels
{
    public class CategoryFormViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength)]
        public string Name { get; set; } = null!;
    }
}
