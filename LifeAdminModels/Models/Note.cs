using System.ComponentModel.DataAnnotations;

namespace LifeAdminModels.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = null!;

        [Required, MaxLength(2000)]
        public string Content { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;
    }
}
