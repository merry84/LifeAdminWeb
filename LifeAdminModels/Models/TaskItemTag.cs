namespace LifeAdminModels.Models
{
    public class TaskItemTag
    {
        public Guid TaskItemId { get; set; }
        public TaskItem TaskItem { get; set; } = null!;

        public Guid TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}