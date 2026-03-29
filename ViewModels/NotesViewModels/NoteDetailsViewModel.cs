namespace ViewModels.NotesViewModels
{
    public class NoteDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
    }
}