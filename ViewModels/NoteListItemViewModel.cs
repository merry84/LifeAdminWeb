namespace ViewModels
{
    public class NoteListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? ContentPreview { get; set; }
    }
}
