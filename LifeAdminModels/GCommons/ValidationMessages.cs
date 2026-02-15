namespace LifeAdminModels.GCommons
{
    public static class ValidationMessages
    {

        public const string RequiredField = "Field is required.";
        public const string ValidStringLength =
            "Field must be between {2} and {1} characters long.";

        public const string CategoryNameRequired = "Category name is required.";
        public const string CategoryNameLength =
            "Category name must be between {2} and {1} characters.";
        public const string CategoryChooseRequired = "Please choose a category.";

        public const string TaskTitleRequired = "Task title is required.";
        public const string TaskTitleLength =
            "Task title must be between {2} and {1} characters.";

        public const string TaskDescriptionLength =
            "Task description must be between {2} and {1} characters.";

        public const string StatusIsRequired = "Status is required.";


        public const string InvalidCategory = "Selected category is invalid.";

        public const string EmailRequired = "Email is required.";
        public const string PasswordRequired = "Password is required.";
        public const string PasswordLength =
            "Password must be at least {2} characters long.";


    }
}
 
