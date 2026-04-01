using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace LearningPlatform.Models
{
    public class ApplicationUser : IdentityUser
    {
        public UserRole? Role { get; set;}
    }
}
