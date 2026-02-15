
using LifeAdminModels.Models;

namespace ViewModels
{
    public class TaskListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public WorkStatus Status { get; set; }

        public DateTime CreatedOn { get; set; }
        public string? OwnerUserName { get; set; }
        public string? OwnerEmail { get; set; }

    }
}
