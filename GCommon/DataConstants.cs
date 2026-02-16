namespace GCommon
{
    public static class DataConstants
    {
        public static class User
        {
            public const int FirstNameMaxLength = 40;
            public const int LastNameMaxLength = 40;
            public const int DisplayNameMaxLength = 40;
            public const int EmailMaxLength = 100;
            public const int PasswordMaxLength = 100;
            public const int PasswordMinLength = 6;
        }

        public static class TaskItem
        {
            
            public const int TitleMaxLength = 50;
            public const int DescriptionMaxLength = 500;
        }

        public static class Category
        {
            public const int NameMaxLength = 50;

        }

        public static class Note
        {
            public const int TitleMaxLength = 60;
            public const int TitleMinLength = 2;
            public const int ContentMaxLength = 2000;
            public const int ContentMinLength = 2;

        }
        public static class CategoryFormViewModel
        {       
            public const int NameMaxLength = 40;
            public const int NameMinLength = 2;

        }
        public static class TaskFormViewModel
        {
            public const int TitleMaxLength = 50;
            
            public const int DescriptionMaxLength = 500;
        }

        public static class ProfileViewModel
        {
            public const int FirstNameMaxLength = 40;
            public const int LastNameMaxLength = 40;
            public const int DisplayNameMaxLength = 40;
        }

    }
}
