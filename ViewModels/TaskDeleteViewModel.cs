using LifeAdminModels.Models;

namespace ViewModels
{
    public class TaskDeleteViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
        public WorkStatus Status { get; set; }
    }
}
