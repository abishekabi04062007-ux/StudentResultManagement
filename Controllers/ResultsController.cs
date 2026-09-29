using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;
using StudentResultManagement.Services;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ResultCalculationService _calculationService;

        public ResultsController(ApplicationDbContext context, ResultCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var students = from s in _context.Students
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                students = students.Where(s => s.RegisterNumber.Contains(searchString)
                                       || s.Name.Contains(searchString));
            }

            return View(await students.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Marks)
                    .ThenInclude(m => m.Subject)
                .FirstOrDefaultAsync(m => m.StudentId == id);

            if (student == null) return NotFound();

            int totalMarks = student.Marks.Sum(m => m.Total);
            int maxMarks = student.Marks.Count * 100;
            
            ViewBag.TotalMarks = totalMarks;
            ViewBag.MaxMarks = maxMarks;
            ViewBag.Percentage = _calculationService.CalculatePercentage(totalMarks, maxMarks);
            ViewBag.PassedSubjects = student.Marks.Count(m => m.ResultStatus == "PASS");
            ViewBag.FailedSubjects = student.Marks.Count(m => m.ResultStatus == "FAIL");
            
            bool isOverallPass = student.Marks.Any() && student.Marks.All(m => m.ResultStatus == "PASS");
            ViewBag.OverallResult = isOverallPass ? "PASS" : "FAIL";

            return View(student);
        }
    }
}
