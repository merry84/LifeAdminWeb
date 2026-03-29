namespace ViewModels.Admin
{
    public class UserDetailsViewModel
    {
        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? UserName { get; set; }

        public string? DisplayName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Bio { get; set; }

        public string? ProfileImageUrl { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsAdmin { get; set; }

        public int TasksCount { get; set; }

        public int NotesCount { get; set; }

        public int DocumentsCount { get; set; }
    }
}