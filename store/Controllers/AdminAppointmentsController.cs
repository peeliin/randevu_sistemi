using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using store.Attributes;
using store.Data;
using store.Models;

namespace store.Controllers
{
    [AdminAuthorize]
    public class AdminAppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AdminAppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index(string? status)
        {
            var appointments = _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .AsQueryable();
            
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<AppointmentStatus>(status, out var statusEnum))
            {
                appointments = appointments.Where(a => a.Status == statusEnum);
            }
            
            var appointmentsList = await appointments
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
            
            ViewBag.StatusFilter = status;
            return View(appointmentsList);
        }
        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();
            
            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (appointment == null)
                return NotFound();
            
            return View(appointment);
        }
        
        public async Task<IActionResult> Create()
        {
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FirstName).ToListAsync(), "Id", "FullName");
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).ToListAsync(), "Id", "Name");
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).ToListAsync(), "Id", "FullName");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.CreatedDate = DateTime.Now;
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu başarıyla oluşturuldu";
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FirstName).ToListAsync(), "Id", "FullName", appointment.CustomerId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).ToListAsync(), "Id", "Name", appointment.ServiceId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", appointment.StaffId);
            return View(appointment);
        }
        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();
            
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();
            
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FirstName).ToListAsync(), "Id", "FullName", appointment.CustomerId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).ToListAsync(), "Id", "Name", appointment.ServiceId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", appointment.StaffId);
            return View(appointment);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Randevu başarıyla güncellendi";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["CustomerId"] = new SelectList(await _context.Customers.OrderBy(c => c.FirstName).ToListAsync(), "Id", "FullName", appointment.CustomerId);
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).ToListAsync(), "Id", "Name", appointment.ServiceId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", appointment.StaffId);
            return View(appointment);
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, AppointmentStatus status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();
            
            appointment.Status = status;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Randevu durumu güncellendi";
            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            
            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(m => m.Id == id);
            
            if (appointment == null)
                return NotFound();
            
            return View(appointment);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu başarıyla silindi";
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}

