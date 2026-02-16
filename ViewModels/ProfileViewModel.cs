using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.ProfileViewModel;
namespace ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(FirstNameMaxLength)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(LastNameMaxLength)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [StringLength(DisplayNameMaxLength)]
        [Display(Name = "Display name")]
        public string? DisplayName { get; set; }

        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
