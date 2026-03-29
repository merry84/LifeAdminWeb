using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.User;

namespace LifeAdminModels.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(DisplayNameMaxLength)]
        public string? DisplayName { get; set; }

        [Required]
        [MaxLength(FirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [Required]
        [MaxLength(LastNameMaxLength)]
        public string LastName { get; set; } = null!;

        [MaxLength(ProfileImageUrlMaxLength)]
        public string? ProfileImageUrl { get; set; }

        [MaxLength(BioMaxLength)]
        public string? Bio { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public ICollection<TaskItem> Tasks { get; set; } = new HashSet<TaskItem>();
        public ICollection<Note> Notes { get; set; } = new HashSet<Note>();

        public ICollection<Document> Documents { get; set; } = new HashSet<Document>();
    }
}
