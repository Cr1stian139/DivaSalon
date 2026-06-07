using Microsoft.AspNetCore.Mvc;

namespace DivaSalon.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(string name, string email, string message)
        {
            // Aici poți implementa trimiterea email-ului
            TempData["SuccessMessage"] = "Mesajul tău a fost trimis! Te vom contacta în curând.";
            return RedirectToAction("Index");
        }
    }
}