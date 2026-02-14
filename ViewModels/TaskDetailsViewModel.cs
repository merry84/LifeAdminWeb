using LifeAdminModels.Models;

namespace ViewModels
{
    public class TaskDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string CategoryName { get; set; } = null!;
        public WorkStatus Status { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
