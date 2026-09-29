using System.ComponentModel.DataAnnotations;

namespace StudentResultManagement.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required]
        [StringLength(20)]
        public string RegisterNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public string Address { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Semester { get; set; }

        public ICollection<Mark> Marks { get; set; } = new List<Mark>();
    }
}
