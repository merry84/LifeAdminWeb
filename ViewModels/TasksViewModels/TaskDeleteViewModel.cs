using LifeAdminModels.Models;

namespace ViewModels.TasksViewModels
{
    public class TaskDeleteViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public WorkStatus Status { get; set; }
    }
}
