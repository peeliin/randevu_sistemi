using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using store.Attributes;
using store.Data;
using store.Models;

namespace store.Controllers
{
    [AdminAuthorize]
    public class AdminCustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AdminCustomersController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(string? search)
        {
            var customers = _context.Customers.AsQueryable();
            
            if (!string.IsNullOrEmpty(search))
            {
                customers = customers.Where(c => 
                    c.FirstName.Contains(search) || 
                    c.LastName.Contains(search) || 
                    c.Phone.Contains(search) ||
                    (c.Email != null && c.Email.Contains(search)));
            }
            
            var customersList = await customers
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
            
            ViewBag.Search = search;
            return View(customersList);
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            
            var customer = await _context.Customers
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Service)
                .Include(c => c.Appointments)
                    .ThenInclude(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (customer == null)
                return NotFound();
            
            return View(customer);
        }
        
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedDate = DateTime.Now;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Müşteri başarıyla eklendi";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
                return NotFound();
            
            return View(customer);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer)
        {
            if (id != customer.Id)
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Müşteri başarıyla güncellendi";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            
            var customer = await _context.Customers
                .Include(c => c.Appointments)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (customer == null)
                return NotFound();
            
            return View(customer);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Müşteri başarıyla silindi";
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}

