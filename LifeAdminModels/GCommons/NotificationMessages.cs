namespace LifeAdminModels.GCommons;

public static class NotificationMessages
{
    public static class Tasks
    {
        public const string Created = "Task created successfully!";
        public const string Updated = "Task updated successfully!";
        public const string Deleted = "Task deleted successfully!";

        public const string CreateFailed = "Something went wrong while creating the task.";
        public const string UpdateFailed = "Unexpected error occurred while updating the task.";
        public const string DeleteFailed = "Unexpected error occurred while deleting the task.";
    }

    public static class Categories
    {
        public const string InvalidCategory = "Invalid category.";
    }
}
