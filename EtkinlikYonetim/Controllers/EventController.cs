using System.Net;
using EtkinlikYonetim.Domain.Entities;
using EtkinlikYonetim.Infrastructure.Context;
using EtkinlikYonetim.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtkinlikYonetim.Controllers
{
    public class EventController : Controller
    {
        private readonly EventDbContext _context;
        private readonly IWebHostEnvironment _env;

        public EventController(EventDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            var events = _context.Events
                .Include(e => e.CreatorUser) 
                .OrderByDescending(e => e.CreatedAt)
                .ThenBy(e => e.StartDate)
                .ToList();

            return View(events);
        }
        public IActionResult List()
        {
            var now = DateTime.Now;

            var events = _context.Events
                .AsNoTracking()
                .Where(e => e.IsActive && e.StartDate >= now)
                .OrderByDescending(e => e.CreatedAt)
                .ThenBy(e => e.StartDate)
                .ToList();

            return View(events);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            var ev = _context.Events
                .Include(e => e.CreatorUser)
                .FirstOrDefault(e => e.Id == id);

            if (ev == null)
                return NotFound();

            var recentEvents = _context.Events
                .Where(e => e.StartDate > DateTime.Now && e.IsActive && e.Id != id)
                .OrderBy(e => e.StartDate)
                .Take(5)
                .ToList();

            ViewBag.RecentEvents = recentEvents;

            return View(ev);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Etkinlik oluşturmak için giriş yapmalısınız.";
                return RedirectToAction("Login", "User");
            }

            return View(new EventFormViewModel
            {
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddHours(1),
                IsActive = true
            });
        }


        [HttpPost]
        public IActionResult Create(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Lütfen tüm gerekli alanları doldurunuz.";
                return View(model);
            }

            if (_context.Events.Any(e => e.Title == model.Title))
            {
                ModelState.AddModelError("Title", "Bu başlıkla bir etkinlik zaten var.");
                return View(model);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["Error"] = "Etkinlik oluşturmak için önce giriş yapmalısınız.";
                return RedirectToAction("Login", "User");
            }

            string? imagePath = null;
            if (model.ImageFile != null)
            {
                var ext = Path.GetExtension(model.ImageFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext) || model.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Yalnızca jpg, jpeg, png uzantılı ve 2MB'dan küçük dosyalar yüklenebilir.");
                    return View(model);
                }

                string fileName = Guid.NewGuid() + ext;
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath)) Directory.CreateDirectory(uploadPath);

                string filePath = Path.Combine(uploadPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                model.ImageFile.CopyTo(stream);

                imagePath = "/uploads/" + fileName;
            }

            var newEvent = new Event
            {
                Title = model.Title,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ShortDescription = model.ShortDescription ?? "",
                LongDescription = WebUtility.HtmlDecode(model.LongDescription), 
                IsActive = model.IsActive,
                ImagePath = imagePath,
                CreatedAt = DateTime.Now,
                CreatorUserId = userId.Value
            };

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            TempData["Success"] = "Etkinlik başarıyla oluşturuldu.";
            return RedirectToAction("List");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var ev = _context.Events.Find(id);
            if (ev == null)
                return NotFound();

            if (ev.CreatorUserId != userId)
                return Forbid();

            var model = new EventFormViewModel
            {
                Id = ev.Id,
                Title = ev.Title,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                ShortDescription = ev.ShortDescription ?? "",
                LongDescription = ev.LongDescription,
                IsActive = ev.IsActive,
                ExistingImagePath = ev.ImagePath
            };

            return View("Edit", model);
        }
        [HttpPost]
        public IActionResult Edit(EventFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Form verileri geçerli değil.";
                return View("Edit", model);
            }

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var ev = _context.Events.Find(model.Id);
            if (ev == null)
                return NotFound();

            if (ev.CreatorUserId != userId)
                return Forbid();

            ev.Title = model.Title;
            ev.StartDate = model.StartDate;
            ev.EndDate = model.EndDate;
            ev.ShortDescription = model.ShortDescription ?? "";
            ev.LongDescription = WebUtility.HtmlDecode(model.LongDescription); // 🔹 HTML decode
            ev.IsActive = model.IsActive;

            if (model.ImageFile != null)
            {
                var ext = Path.GetExtension(model.ImageFile.FileName).ToLower();
                var allowed = new[] { ".jpg", ".jpeg", ".png" };

                if (!allowed.Contains(ext) || model.ImageFile.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImageFile", "Yalnızca JPG, JPEG veya PNG uzantılı ve 2MB'dan küçük dosyalar kabul edilir.");
                    return View("Edit", model);
                }

                if (!string.IsNullOrEmpty(ev.ImagePath))
                {
                    var oldPath = Path.Combine(_env.WebRootPath, ev.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                string fileName = Guid.NewGuid() + ext;
                string uploadPath = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                string filePath = Path.Combine(uploadPath, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                model.ImageFile.CopyTo(stream);

                ev.ImagePath = "/uploads/" + fileName;
            }

            _context.SaveChanges();
            TempData["Success"] = "Etkinlik başarıyla güncellendi.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "User");

            var ev = _context.Events.Find(id);
            if (ev == null)
                return NotFound();

            if (ev.CreatorUserId != userId)
                return Forbid();

            _context.Events.Remove(ev);
            _context.SaveChanges();

            TempData["Success"] = "Etkinlik başarıyla silindi.";
            return RedirectToAction("Index");
        }
    }
}
