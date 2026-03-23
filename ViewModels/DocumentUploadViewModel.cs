using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.Document;

namespace ViewModels
{
    public class DocumentUploadViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}