using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.Note;
namespace LifeAdminModels.Models
{
    public class Note
    {
        public int Id { get; set; }

        [Required, MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required, MaxLength(ContentMaxLength)]
        public string Content { get; set; } = null!;

        [Required]
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;
    }
}
