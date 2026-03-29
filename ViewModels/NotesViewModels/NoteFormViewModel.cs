using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.Note;
namespace ViewModels.NotesViewModels
{
    public class NoteFormViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(ContentMaxLength, MinimumLength = ContentMinLength)]
        public string Content { get; set; } = null!;
    }
}
