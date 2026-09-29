using System.ComponentModel.DataAnnotations;

namespace StudentResultManagement.Models
{
    public class Subject
    {
        public int SubjectId { get; set; }

        [Required]
        [StringLength(20)]
        public string SubjectCode { get; set; }

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Semester { get; set; }

        [Required]
        public int Credits { get; set; }

        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
    }
}
