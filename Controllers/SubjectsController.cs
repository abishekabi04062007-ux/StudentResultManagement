using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentResultManagement.Data;
using StudentResultManagement.Models;

namespace StudentResultManagement.Controllers
{
    [Authorize]
    public class SubjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SubjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Subjects.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Subject subject)
        {
            ModelState.Remove("Marks");
            if (await _context.Subjects.AnyAsync(s => s.SubjectCode == subject.SubjectCode))
            {
                ModelState.AddModelError("SubjectCode", "Subject Code must be unique.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Subject subject)
        {
            if (id != subject.SubjectId) return NotFound();

            ModelState.Remove("Marks");
            if (await _context.Subjects.AnyAsync(s => s.SubjectCode == subject.SubjectCode && s.SubjectId != id))
            {
                ModelState.AddModelError("SubjectCode", "Subject Code must be unique.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Subjects.Any(e => e.SubjectId == subject.SubjectId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(subject);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var subject = await _context.Subjects.FirstOrDefaultAsync(m => m.SubjectId == id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var subject = await _context.Subjects.FirstOrDefaultAsync(m => m.SubjectId == id);
            if (subject == null) return NotFound();
            return View(subject);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hasMarks = await _context.Marks.AnyAsync(m => m.SubjectId == id);
            if (hasMarks)
            {
                TempData["Error"] = "This subject cannot be deleted because marks are already assigned to it.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Subject deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
