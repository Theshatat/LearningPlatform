namespace LearningPlatform.Models
{
    public class Enrollment: BaseEntity
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public double Progress { get; set; }
        public User Student { get; set; }
        public Course Course { get; set; }
    }
}
