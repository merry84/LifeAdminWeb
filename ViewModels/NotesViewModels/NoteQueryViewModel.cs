namespace ViewModels.NotesViewModels
{
    public class NoteQueryViewModel
    {
        public string? SearchTerm { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int NotesPerPage { get; set; } = 5;

        public int TotalNotesCount { get; set; }

        public IEnumerable<NoteListItemViewModel> Notes { get; set; } = new List<NoteListItemViewModel>();
    }
}