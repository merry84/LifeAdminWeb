namespace ViewModels
{
    public class TaskQueryViewModel
    {
        public string? SearchTerm { get; set; }

        public int? CategoryId { get; set; }

        public int? Status { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int TasksPerPage { get; set; } = 5;

        public int TotalTasksCount { get; set; }

        public IEnumerable<TaskListViewModel> Tasks { get; set; } = new List<TaskListViewModel>();

        public IEnumerable<TaskCategoryOptionViewModel> Categories { get; set; } = new List<TaskCategoryOptionViewModel>();
    }
}