using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;
using StudentResultManagement.Models.ViewModels;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var viewModel = new DashboardViewModel();

            viewModel.TotalStudents = await _context.Students.CountAsync();
            viewModel.TotalSubjects = await _context.Subjects.CountAsync();
            viewModel.TotalMarks = await _context.Marks.CountAsync();

            var studentsWithMarks = await _context.Students
                .Include(s => s.Marks)
                .Where(s => s.Marks.Any())
                .ToListAsync();

            int passed = 0;
            int failed = 0;

            foreach (var student in studentsWithMarks)
            {
                bool isFail = student.Marks.Any(m => m.ResultStatus == "FAIL");
                if (isFail) failed++;
                else passed++;
            }

            viewModel.PassedStudents = passed;
            viewModel.FailedStudents = failed;

            var marks = await _context.Marks.ToListAsync();
            
            viewModel.GradeDistribution = new Dictionary<string, int>
            {
                { "A+", marks.Count(m => m.Grade == "A+") },
                { "A", marks.Count(m => m.Grade == "A") },
                { "B+", marks.Count(m => m.Grade == "B+") },
                { "B", marks.Count(m => m.Grade == "B") },
                { "C", marks.Count(m => m.Grade == "C") },
                { "D", marks.Count(m => m.Grade == "D") },
                { "F", marks.Count(m => m.Grade == "F") }
            };

            viewModel.RecentStudents = await _context.Students
                .OrderByDescending(s => s.StudentId)
                .Take(5)
                .ToListAsync();

            return View(viewModel);
        }
    }
}
