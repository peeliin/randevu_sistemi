using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using store.Attributes;
using store.Data;
using store.Services;

namespace store.Controllers
{
    [AdminAuthorize]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AdminAuthService _authService;
        
        public AdminController(ApplicationDbContext context, AdminAuthService authService)
        {
            _context = context;
            _authService = authService;
        }
        
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (_authService.IsAuthenticated())
                return RedirectToAction("Dashboard");
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Kullanıcı adı ve şifre gereklidir";
                return View();
            }
            
            var result = await _authService.LoginAsync(username, password);
            if (result)
            {
                return RedirectToAction("Dashboard");
            }
            
            ViewBag.Error = "Kullanıcı adı veya şifre hatalı";
            return View();
        }
        
        public IActionResult Logout()
        {
            _authService.Logout();
            return RedirectToAction("Login");
        }
        
        public async Task<IActionResult> Dashboard()
        {
            var stats = new
            {
                TotalAppointments = await _context.Appointments.CountAsync(),
                PendingAppointments = await _context.Appointments.CountAsync(a => a.Status == Models.AppointmentStatus.Pending),
                TotalCustomers = await _context.Customers.CountAsync(),
                TotalServices = await _context.Services.CountAsync(s => s.IsActive),
                TotalStaff = await _context.Staff.CountAsync(s => s.IsActive),
                TodayAppointments = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate.Date == DateTime.Today)
            };
            
            var recentAppointments = await _context.Appointments
                .Include(a => a.Customer)
                .Include(a => a.Service)
                .Include(a => a.Staff)
                .OrderByDescending(a => a.AppointmentDate)
                .Take(10)
                .ToListAsync();
            
            ViewBag.Stats = stats;
            ViewBag.RecentAppointments = recentAppointments;
            
            return View();
        }
    }
}
