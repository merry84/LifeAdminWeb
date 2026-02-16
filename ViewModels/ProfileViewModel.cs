using System.ComponentModel.DataAnnotations;

namespace ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(40, MinimumLength = 2)]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = null!;

        [StringLength(40)]
        [Display(Name = "Display name")]
        public string? DisplayName { get; set; }

        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
