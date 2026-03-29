namespace ViewModels.DocumentViewModels
{
    public class DocumentDeleteViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string FileName { get; set; } = null!;
    }
}