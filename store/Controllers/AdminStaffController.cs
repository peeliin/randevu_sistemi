using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using store.Attributes;
using store.Data;
using store.Models;

namespace store.Controllers
{
    [AdminAuthorize]
    public class AdminStaffController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AdminStaffController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var staff = await _context.Staff.OrderBy(s => s.FirstName).ToListAsync();
            return View(staff);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Staff staff)
        {
            if (ModelState.IsValid)
            {
                staff.CreatedDate = DateTime.Now;
                _context.Add(staff);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Personel başarıyla eklendi";
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null)
                return NotFound();
            
            return View(staff);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Staff staff)
        {
            if (id != staff.Id)
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(staff);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Personel başarıyla güncellendi";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StaffExists(staff.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(staff);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            
            var staff = await _context.Staff
                .Include(s => s.Appointments)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (staff == null)
                return NotFound();
            
            return View(staff);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff != null)
            {
                _context.Staff.Remove(staff);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Personel başarıyla silindi";
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool StaffExists(int id)
        {
            return _context.Staff.Any(e => e.Id == id);
        }
    }
}

