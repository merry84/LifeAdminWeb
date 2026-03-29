namespace ViewModels.DashboardViewModels
{
    public class DashboardStatsViewModel
    {
        public int MyTasksCount { get; set; }
        public int MyNotesCount { get; set; }
        public int CategoriesCount { get; set; }

        public int TotalUsers { get; set; }
        public int TotalTasks { get; set; }
        public int TotalNotes { get; set; }
    }
}