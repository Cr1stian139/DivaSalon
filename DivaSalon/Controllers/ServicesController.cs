using DivaSalon.Data;
using DivaSalon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DivaSalon.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services.Where(s => s.IsActive).ToListAsync();
            return View(services);
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceDetails(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
                return NotFound();

            return Json(new
            {
                name = service.Name,
                category = service.Category,
                priceFrom = service.PriceFrom,
                priceTo = service.PriceTo
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPopularServices()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .Take(3)
                .ToListAsync();
            return Json(services);
        }
    }
}