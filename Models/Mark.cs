using System.ComponentModel.DataAnnotations;

namespace StudentResultManagement.Models
{
    public class Mark
    {
        public int MarkId { get; set; }

        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        [Required]
        [Range(0, 30)]
        public int InternalMark { get; set; }

        [Required]
        [Range(0, 70)]
        public int ExternalMark { get; set; }

        public int Total { get; set; }
        public string Grade { get; set; }
        public string ResultStatus { get; set; }
    }
}
