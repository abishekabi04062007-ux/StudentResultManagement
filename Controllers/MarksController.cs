using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;
using StudentResultManagement.Models;
using StudentResultManagement.Services;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ResultCalculationService _calculationService;

        public MarksController(ApplicationDbContext context, ResultCalculationService calculationService)
        {
            _context = context;
            _calculationService = calculationService;
        }

        public async Task<IActionResult> Index()
        {
            var marks = await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .ToListAsync();
            return View(marks);
        }

        public IActionResult Create()
        {
            PrepareDropdowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Mark mark)
        {
            // Remove navigational properties from validation
            ModelState.Remove("Student");
            ModelState.Remove("Subject");
            ModelState.Remove("Grade");
            ModelState.Remove("ResultStatus");

            var exists = await _context.Marks.AnyAsync(m => m.StudentId == mark.StudentId && m.SubjectId == mark.SubjectId);
            if (exists)
            {
                ModelState.AddModelError("", "Marks for this student and subject already exist.");
            }

            if (mark.InternalMark < 0 || mark.InternalMark > 30)
                ModelState.AddModelError("InternalMark", "Internal Mark must be between 0 and 30.");
            
            if (mark.ExternalMark < 0 || mark.ExternalMark > 70)
                ModelState.AddModelError("ExternalMark", "External Mark must be between 0 and 70.");

            if (ModelState.IsValid)
            {
                mark.Total = _calculationService.CalculateTotal(mark.InternalMark, mark.ExternalMark);
                mark.Grade = _calculationService.CalculateGrade(mark.Total);
                mark.ResultStatus = _calculationService.CalculateResultStatus(mark.Total);

                _context.Add(mark);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marks saved successfully!";
                return RedirectToAction(nameof(Index));
            }

            PrepareDropdowns(mark.StudentId, mark.SubjectId);
            return View(mark);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _context.Marks.FindAsync(id);
            if (mark == null) return NotFound();

            PrepareDropdowns(mark.StudentId, mark.SubjectId);
            return View(mark);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Mark mark)
        {
            if (id != mark.MarkId) return NotFound();

            ModelState.Remove("Student");
            ModelState.Remove("Subject");
            ModelState.Remove("Grade");
            ModelState.Remove("ResultStatus");

            // Check for duplicates excluding current record
            var exists = await _context.Marks.AnyAsync(m => m.StudentId == mark.StudentId && m.SubjectId == mark.SubjectId && m.MarkId != id);
            if (exists)
            {
                ModelState.AddModelError("", "Marks for this student and subject already exist.");
            }

            if (mark.InternalMark < 0 || mark.InternalMark > 30)
                ModelState.AddModelError("InternalMark", "Internal Mark must be between 0 and 30.");
            
            if (mark.ExternalMark < 0 || mark.ExternalMark > 70)
                ModelState.AddModelError("ExternalMark", "External Mark must be between 0 and 70.");

            if (ModelState.IsValid)
            {
                try
                {
                    mark.Total = _calculationService.CalculateTotal(mark.InternalMark, mark.ExternalMark);
                    mark.Grade = _calculationService.CalculateGrade(mark.Total);
                    mark.ResultStatus = _calculationService.CalculateResultStatus(mark.Total);

                    _context.Update(mark);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Marks updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(mark.MarkId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PrepareDropdowns(mark.StudentId, mark.SubjectId);
            return View(mark);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var mark = await _context.Marks
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.MarkId == id);
            
            if (mark == null) return NotFound();

            return View(mark);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Marks.FindAsync(id);
            if (mark != null)
            {
                _context.Marks.Remove(mark);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Marks deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool MarkExists(int id)
        {
            return _context.Marks.Any(e => e.MarkId == id);
        }

        private void PrepareDropdowns(int? selectedStudent = null, int? selectedSubject = null)
        {
            var students = _context.Students.Select(s => new { 
                StudentId = s.StudentId, 
                DisplayName = s.RegisterNumber + " - " + s.Name 
            }).ToList();
            
            var subjects = _context.Subjects.Select(s => new { 
                SubjectId = s.SubjectId, 
                DisplayName = s.SubjectCode + " - " + s.SubjectName 
            }).ToList();
            
            ViewBag.StudentId = new SelectList(students, "StudentId", "DisplayName", selectedStudent);
            ViewBag.SubjectId = new SelectList(subjects, "SubjectId", "DisplayName", selectedSubject);
        }
    }
}
