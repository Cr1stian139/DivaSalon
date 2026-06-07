using DivaSalon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DivaSalon.Controllers
{
    public class BarbersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarbersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var barbers = await _context.Barbers
                .Where(b => b.IsActive)
                .ToListAsync();
            return View(barbers);
        }
    }
}