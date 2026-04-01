using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Models
{
    public class User:BaseEntity
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public List <Course> Courses { get; set; }
        public List<Enrollment> Enrollments { get; set; }
    }
}
