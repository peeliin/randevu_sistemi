using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using store.Data;
using store.Models;

namespace store.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public AppointmentController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Create()
        {
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).ToListAsync(), "Id", "Name");
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).ToListAsync(), "Id", "FullName");
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if customer exists by phone
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Phone == model.Phone);
                
                if (customer == null)
                {
                    // Create new customer
                    customer = new Customer
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Phone = model.Phone,
                        Email = model.Email,
                        CreatedDate = DateTime.Now
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update existing customer info if needed
                    customer.FirstName = model.FirstName;
                    customer.LastName = model.LastName;
                    if (!string.IsNullOrEmpty(model.Email))
                        customer.Email = model.Email;
                    _context.Update(customer);
                }
                
                // Create appointment
                var appointment = new Appointment
                {
                    CustomerId = customer.Id,
                    ServiceId = model.ServiceId,
                    StaffId = model.StaffId,
                    AppointmentDate = model.AppointmentDate,
                    Status = AppointmentStatus.Pending,
                    Notes = model.Notes,
                    CreatedDate = DateTime.Now
                };
                
                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Randevunuz başarıyla oluşturuldu. En kısa sürede sizinle iletişime geçeceğiz.";
                return RedirectToAction("Success", new { id = appointment.Id });
            }
            
            ViewData["ServiceId"] = new SelectList(await _context.Services.Where(s => s.IsActive).ToListAsync(), "Id", "Name", model.ServiceId);
            ViewData["StaffId"] = new SelectList(await _context.Staff.Where(s => s.IsActive).ToListAsync(), "Id", "FullName", model.StaffId);
            return View(model);
        }
        
        public async Task<IActionResult> Success(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .FirstOrDefaultAsync(a => a.Id == id);
            
            if (appointment == null)
                return NotFound();
            
            return View(appointment);
        }
    }
    
    public class AppointmentViewModel
    {
        [Required(ErrorMessage = "Ad gereklidir")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Soyad gereklidir")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Telefon gereklidir")]
        [Display(Name = "Telefon")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; } = string.Empty;
        
        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }
        
        [Required(ErrorMessage = "Servis gereklidir")]
        [Display(Name = "Servis")]
        public int ServiceId { get; set; }
        
        [Display(Name = "Personel")]
        public int? StaffId { get; set; }
        
        [Required(ErrorMessage = "Randevu tarihi gereklidir")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
    }
}

