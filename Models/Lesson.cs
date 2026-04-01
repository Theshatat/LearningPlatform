using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.Models
{
    public class Lesson: BaseEntity
    {
        public int Id { get; set; }
        [MaxLength(50)]
        [Required]
        public string Title { get; set; }
        public string VideoURL { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
