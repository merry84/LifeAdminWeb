using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.ProfileViewModel;
namespace ViewModels.ProfilesViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = null!;

        [Display(Name = "Username")]
        public string UserName { get; set; } = null!;

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

        [Display(Name = "Profile Image URL")]
        [StringLength(ProfileImageUrlMaxLength)]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Bio")]
        [StringLength(BioMaxLength)]
        public string? Bio { get; set; }


        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
