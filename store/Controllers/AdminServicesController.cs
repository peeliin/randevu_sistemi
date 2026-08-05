using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using store.Attributes;
using store.Data;
using store.Models;

namespace store.Controllers
{
    [AdminAuthorize]
    public class AdminServicesController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AdminServicesController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.OrderBy(s => s.Name).ToListAsync();
            return View(services);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            if (ModelState.IsValid)
            {
                service.CreatedDate = DateTime.Now;
                _context.Add(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Servis başarıyla eklendi";
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();
            
            return View(service);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id)
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(service);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Servis başarıyla güncellendi";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ServiceExists(service.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(service);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();
            
            return View(service);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Servis başarıyla silindi";
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool ServiceExists(int id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}

