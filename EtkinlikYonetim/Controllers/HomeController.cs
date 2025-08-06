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
            // Session'dan kullanıcı adı alınır
            ViewBag.FullName = HttpContext.Session.GetString("FullName") ?? "Misafir Kullanıcı";

            // Şu andan sonraki ve aktif etkinlikler alınır
            var etkinlikler = _context.Events
                .Where(e => e.StartDate >= DateTime.Now && e.IsActive)
                .OrderBy(e => e.StartDate)
                .ToList();

            // Debug amaçlı sayıya bak
            ViewBag.Count = etkinlikler.Count;

            return View(etkinlikler);
        }
    }
}
