using EtkinlikYonetim.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace EtkinlikYonetim.Controllers
{
    public class CalendarController : Controller
    {
        private readonly EventDbContext _context;

        public CalendarController(EventDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetEvents()
        {
            var events = _context.Events
                .Where(e => e.StartDate >= DateTime.Now && e.IsActive)
                .Select(e => new
                {
                    id = e.Id,
                    title = $"{e.Title} ({e.StartDate:HH:mm} - {e.EndDate:HH:mm})",
                    start = e.StartDate.ToString("s"),
                    end = e.EndDate.ToString("s")
                }).ToList();

            return Json(events);
        }

    }
}
