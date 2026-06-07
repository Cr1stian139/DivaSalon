using DivaSalon.Data;
using DivaSalon.Models;
using DivaSalon.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DivaSalon.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public OrdersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var viewModel = new OrderViewModel
            {
                AvailableServices = await _context.Services.Where(s => s.IsActive).ToListAsync(),
                AvailableBarbers = await _context.Barbers.Where(b => b.IsActive).ToListAsync(),
                Order = new Order(),
                SelectedServiceIds = new List<int>()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderViewModel viewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var selectedServices = await _context.Services
                .Where(s => viewModel.SelectedServiceIds.Contains(s.Id))
                .ToListAsync();

            if (!selectedServices.Any())
            {
                TempData["Error"] = "Te rugam sa selectezi cel putin un serviciu!";
                return RedirectToAction("Create");
            }

            // Validare data si ora
            if (viewModel.Order.AppointmentDate == default(DateTime))
            {
                TempData["Error"] = "Te rugam sa selectezi o data si ora valida!";
                return RedirectToAction("Create");
            }

            // Verifica daca data este in trecut
            if (viewModel.Order.AppointmentDate < DateTime.Now)
            {
                TempData["Error"] = "Nu poti programa o data in trecut!";
                return RedirectToAction("Create");
            }

            // Calculeaza totalul minim si maxim
            var totalMin = selectedServices.Sum(s => s.PriceFrom);
            var totalMax = selectedServices.Sum(s => s.PriceTo ?? s.PriceFrom);

            // Pentru suma finala in comanda, folosim maximul
            var totalAmount = totalMax;

            var order = new Order
            {
                UserId = user.Id,
                CustomerName = string.IsNullOrEmpty(viewModel.Order.CustomerName) ? user.Email ?? "Client" : viewModel.Order.CustomerName,
                CustomerPhone = viewModel.Order.CustomerPhone ?? "",
                AppointmentDate = viewModel.Order.AppointmentDate,
                BarberId = viewModel.Order.BarberId,
                TotalAmount = totalAmount,
                Notes = viewModel.Order.Notes ?? "",
                CreatedAt = DateTime.Now,
                Status = OrderStatus.Pending,
                OrderItems = selectedServices.Select(s => new OrderItem
                {
                    ServiceId = s.Id,
                    ServiceName = s.Name,
                    Price = s.PriceTo ?? s.PriceFrom
                }).ToList()
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Programarea ta a fost inregistrata cu succes!";
            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public async Task<IActionResult> MyOrders()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var orders = await _context.Orders
                .Include(o => o.Barber)
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == user.Id)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null && order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.Cancelled;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("MyOrders");
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableTimes(int barberId, string date)
        {
            var selectedDate = DateTime.Parse(date);
            var startOfDay = selectedDate.Date;
            var endOfDay = selectedDate.Date.AddDays(1).AddSeconds(-1);

            // Toate orele disponibile (din 30 in 30 minute)
            var allTimes = new List<string>();
            for (int hour = 9; hour <= 20; hour++)
            {
                allTimes.Add($"{hour:D2}:00");
                if (hour != 20)
                {
                    allTimes.Add($"{hour:D2}:30");
                }
            }

            // Orele deja ocupate pentru acest specialist in aceasta zi
            var bookedOrders = await _context.Orders
                .Where(o => o.BarberId == barberId
                            && o.AppointmentDate >= startOfDay
                            && o.AppointmentDate <= endOfDay
                            && o.Status != OrderStatus.Cancelled)
                .Select(o => o.AppointmentDate.ToString("HH:mm"))
                .ToListAsync();

            // Filtreaza orele ocupate
            var availableTimes = allTimes.Except(bookedOrders).ToList();

            return Json(availableTimes);
        }
    }
}