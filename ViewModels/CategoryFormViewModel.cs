using System.ComponentModel.DataAnnotations;


namespace ViewModels
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Name { get; set; } = null!;
    }
}
