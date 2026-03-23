namespace ViewModels
{
    public class DocumentListViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public long FileSize { get; set; }
        public DateTime UploadedOn { get; set; }
        public string? OwnerUserName { get; set; }
    }
}