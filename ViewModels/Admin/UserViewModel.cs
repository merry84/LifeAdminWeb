namespace ViewModels.Admin
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? DisplayName { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime CreatedOn { get; set; }

        public bool IsAdmin { get; set; }
    }
}