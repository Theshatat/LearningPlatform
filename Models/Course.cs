using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Models
{
    public class Course: BaseEntity
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [StringLength(250)]
        public string Description { get; set; }
        public int InstructorId { get; set; }
        public bool IsPublished { get; set; }
        public User Instructor { get; set; }
        public List<Lesson> Lessons { get; set; }
        public List<Enrollment> Enrollments { get; set; }
    }
}
