using EtkinlikYonetim.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EtkinlikYonetim.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventDbContext _context;

        public HomeController(EventDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.FullName = HttpContext.Session.GetString("UserName") ?? "Misafir Kullanıcı";

            var etkinlikler = _context.Events
                .Where(e => e.StartDate >= DateTime.Now && e.IsActive)
                .OrderBy(e => e.StartDate)
                .ToList();

            return View(etkinlikler);
        }
    }
}
