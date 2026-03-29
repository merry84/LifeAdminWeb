using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.Document;

namespace LifeAdminModels.Models
{
    public class Document
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(FileNameMaxLength)]
        public string FileName { get; set; } = null!;

        [Required]
        [MaxLength(StoredFileNameMaxLength)]
        public string StoredFileName { get; set; } = null!;

        [Required]
        [MaxLength(ContentTypeMaxLength)]
        public string ContentType { get; set; } = null!;

        public long FileSize { get; set; }

        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        [Required]
        public string OwnerId { get; set; } = null!;
        public ApplicationUser Owner { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}