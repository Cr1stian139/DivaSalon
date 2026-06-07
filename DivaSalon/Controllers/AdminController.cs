using DivaSalon.Data;
using DivaSalon.Models;
using DivaSalon.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DivaSalon.Controllers
{
    [Authorize(Roles = "Admin,Barber")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Dashboard()
        {
            var allOrders = await _context.Orders
                .Include(o => o.Barber)
                .Include(o => o.OrderItems)
                .Include(o => o.User)
                .OrderByDescending(o => o.AppointmentDate)
                .ToListAsync();

            var viewModel = new AdminOrderViewModel
            {
                PendingOrders = allOrders.Where(o => o.Status == OrderStatus.Pending).ToList(),
                ConfirmedOrders = allOrders.Where(o => o.Status == OrderStatus.Confirmed).ToList(),
                InProgressOrders = allOrders.Where(o => o.Status == OrderStatus.InProgress).ToList(),
                CompletedOrders = allOrders.Where(o => o.Status == OrderStatus.Completed).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Dashboard");
        }

        // Barbers management
        public async Task<IActionResult> Barbers()
        {
            var barbers = await _context.Barbers.ToListAsync();
            return View(barbers);
        }

        [HttpPost]
        public async Task<IActionResult> AddBarber(Barber barber)
        {
            if (ModelState.IsValid)
            {
                barber.IsActive = true;
                _context.Barbers.Add(barber);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Specialist adaugat cu succes!";
            }
            return RedirectToAction("Barbers");
        }

        [HttpPost]
        public async Task<IActionResult> EditBarber(Barber barber)
        {
            if (ModelState.IsValid)
            {
                _context.Barbers.Update(barber);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Specialist actualizat cu succes!";
            }
            return RedirectToAction("Barbers");
        }

        [HttpGet]
        public async Task<IActionResult> GetBarber(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            return Json(barber);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleBarberStatus(int id)
        {
            var barber = await _context.Barbers.FindAsync(id);
            if (barber != null)
            {
                barber.IsActive = !barber.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Barbers");
        }

        // Services management
        public async Task<IActionResult> Services()
        {
            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        [HttpPost]
        public async Task<IActionResult> AddService(Service service)
        {
            if (ModelState.IsValid)
            {
                if (service.PriceTo == 0)
                {
                    service.PriceTo = null;
                }
                service.IsActive = true;
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Serviciu adaugat cu succes!";
            }
            return RedirectToAction("Services");
        }

        [HttpPost]
        public async Task<IActionResult> EditService(Service service)
        {
            if (ModelState.IsValid)
            {
                if (service.PriceTo == 0)
                {
                    service.PriceTo = null;
                }
                _context.Services.Update(service);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Serviciu actualizat cu succes!";
            }
            return RedirectToAction("Services");
        }

        [HttpGet]
        public async Task<IActionResult> GetService(int id)
        {
            var service = await _context.Services.FindAsync(id);
            return Json(service);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleServiceStatus(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                service.IsActive = !service.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Services");
        }
    }
}