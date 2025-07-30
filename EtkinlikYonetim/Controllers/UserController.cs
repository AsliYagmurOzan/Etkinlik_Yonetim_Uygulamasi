using EtkinlikYonetim.Application.Services.Security;
using EtkinlikYonetim.Domain.Entities;
using EtkinlikYonetim.Infrastructure.Context;
using EtkinlikYonetim.Models;
using Microsoft.AspNetCore.Mvc;

namespace EtkinlikYonetim.Controllers
{
    public class UserController : Controller
    {
        private readonly EventDbContext _context;
        private readonly PasswordHasher _passwordHasher;

        public UserController(EventDbContext context)
        {
            _context = context;
            _passwordHasher = new PasswordHasher();
        }

        //Kayıt alma
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UserViewModel model)
        {
            if (!ModelState.IsValid || model.DateOfBirth == null)
            {
                if (model.DateOfBirth == null)
                    ModelState.AddModelError("DateOfBirth", "Doğum tarihi zorunludur.");
                return View(model);
            }

            if (_context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            var hashedPassword = _passwordHasher.HashPassword(model.Password);

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = hashedPassword,
                DateOfBirth = model.DateOfBirth.Value,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Kayıt başarılı!";
            return RedirectToAction("Login");
        }

        //Giriş
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !_passwordHasher.VerifyPassword(model.Password, user.PasswordHash))
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya parola.");
                return View(model);
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Index", "Home");
        }
        //Çıkış
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
