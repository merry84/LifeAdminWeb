using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.CategoryFormViewModel;

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
