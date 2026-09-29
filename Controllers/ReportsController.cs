using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> SemesterReport(string department, int? year, int? semester)
        {
            var students = await _context.Students
                .Include(s => s.Marks)
                .Where(s => (string.IsNullOrEmpty(department) || s.Department == department) &&
                            (!year.HasValue || s.Year == year) &&
                            (!semester.HasValue || s.Semester == semester))
                .ToListAsync();

            return View(students);
        }
    }
}
