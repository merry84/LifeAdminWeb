using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using LifeAdminModels.Models;

using static GCommon.DataConstants.TaskFormViewModel;
using static GCommon.ValidationMessages;
namespace ViewModels
{
    public class TaskFormViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Display(Name = "Category")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }

        [Display(Name = "Status")]
        public WorkStatus Status { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; } 
            = new List<SelectListItem>();
    }
}
