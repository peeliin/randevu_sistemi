using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using store.Data;
using store.Models;

namespace store.Services
{
    public class AdminAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        private const string SessionKey = "AdminUserId";
        
        public AdminAuthService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<bool> LoginAsync(string username, string password)
        {
            var admin = await _context.AdminUsers
                .FirstOrDefaultAsync(a => a.Username == username && a.IsActive);
            
            if (admin == null)
                return false;
            
            // Simple password check (in production, use proper hashing)
            if (admin.Password == password)
            {
                _httpContextAccessor.HttpContext?.Session.SetInt32(SessionKey, admin.Id);
                return true;
            }
            
            return false;
        }
        
        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Remove(SessionKey);
        }
        
        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32(SessionKey) != null;
        }
        
        public int? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.Session.GetInt32(SessionKey);
        }
        
        public async Task<AdminUser?> GetCurrentUserAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return null;
                
            return await _context.AdminUsers.FindAsync(userId);
        }
    }
}

