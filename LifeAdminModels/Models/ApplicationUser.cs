using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LifeAdminModels.Models
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(40)]
        public string? DisplayName { get; set; }

       
         public string FirstName { get; set; } = null!;
         
        public string LastName { get; set; } = null!;
        

    }
}
