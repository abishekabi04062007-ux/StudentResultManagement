namespace StudentResultManagement.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalSubjects { get; set; }
        public int TotalMarks { get; set; }
        public int PassedStudents { get; set; }
        public int FailedStudents { get; set; }
        
        public Dictionary<string, int> GradeDistribution { get; set; } = new Dictionary<string, int>();
        public List<Student> RecentStudents { get; set; } = new List<Student>();
    }
}
