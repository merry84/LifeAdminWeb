using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using static GCommon.DataConstants.User;

public class ApplicationUser : IdentityUser
{
    [MaxLength(DisplayNameMaxLength)]
    public string? DisplayName { get; set; }

    [Required]
    [MaxLength(FirstNameMaxLength)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(LastNameMaxLength)]
    public string LastName { get; set; } = null!;
}
