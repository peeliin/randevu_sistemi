using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using store.Data;
using store.Models;

namespace store.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }
        
        
        public async Task<IActionResult> Index()
        {
            var objCategoryList = await _db.Categories.ToListAsync();
            return View(objCategoryList);
        }
    }
}
