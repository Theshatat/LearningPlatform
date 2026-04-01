using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Models
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Email { get; set; }
        public UserRole Role { get; set; }
    }
}
