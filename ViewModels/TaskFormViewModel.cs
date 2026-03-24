using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using LifeAdminModels.Models;

using static GCommon.DataConstants.TaskFormViewModel;
using static GCommon.ValidationMessages;
namespace ViewModels
{
    public class TaskFormViewModel
    {
        public Guid Id { get; set; }

        [Required, MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(DescriptionMaxLength)]
        public string? Description { get; set; }

        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select a category.")]
        public Guid? CategoryId { get; set; }

        [Display(Name = "Status")]
        public WorkStatus Status { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; } 
            = new List<SelectListItem>();

        public List<Guid> SelectedTagIds { get; set; } = new();
        public List<TagOptionViewModel> Tags { get; set; } = new();
    }
}
