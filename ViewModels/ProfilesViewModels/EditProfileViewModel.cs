using System.ComponentModel.DataAnnotations;

namespace ViewModels.ProfilesViewModels
{
    public class EditProfileViewModel
    {
        [Display(Name = "Display Name")]
        [StringLength(40)]
        public string? DisplayName { get; set; }

        [Display(Name = "First Name")]
        [StringLength(30)]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(30)]
        public string? LastName { get; set; }

        [Display(Name = "Profile Image URL")]
        [StringLength(250)]
        [Url]
        public string? ProfileImageUrl { get; set; }

        [Display(Name = "Bio")]
        [StringLength(500)]
        public string? Bio { get; set; }

        [Display(Name = "Phone Number")]
        [Phone]
        public string? PhoneNumber { get; set; }
    }
}