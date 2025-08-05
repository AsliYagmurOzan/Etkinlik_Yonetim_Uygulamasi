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
        private readonly PasswordEncryptor _passwordEncryptor;

        public UserController(EventDbContext context)
        {
            _context = context;
            _passwordEncryptor = new PasswordEncryptor();
        }

        // Kayıt alma
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

            var encryptedPassword = _passwordEncryptor.Encrypt(model.Password);

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                EncryptedPassword = encryptedPassword,
                DateOfBirth = model.DateOfBirth.Value,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Kayıt başarılı!";
            return RedirectToAction("Login");
        }

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
            if (user == null)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya parola.");
                return View(model);
            }

            string decryptedPassword;
            try
            {
                decryptedPassword = _passwordEncryptor.Decrypt(user.EncryptedPassword);
            }
            catch
            {
                ModelState.AddModelError("", "Parola doğrulama sırasında bir hata oluştu.");
                return View(model);
            }

            if (model.Password != decryptedPassword)
            {
                ModelState.AddModelError("", "Geçersiz e-posta veya parola.");
                return View(model);
            }

            // ✅ Session bilgilerini set et
            HttpContext.Session.SetInt32("UserId", user.Id); // ← BU SATIR EKLENDİ
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            var model = new UpdateProfileViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                BirthDate = user.DateOfBirth,
                CurrentPassword = "",
                NewPassword = ""
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult Profile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            var currentDecrypted = _passwordEncryptor.Decrypt(user.EncryptedPassword);
            if (model.CurrentPassword != currentDecrypted)
            {
                ModelState.AddModelError("CurrentPassword", "Mevcut parola yanlış.");
                return View(model);
            }

            if (model.Email != user.Email &&
                _context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu mail adresi başka bir kullanıcıya ait.");
                return View(model);
            }

            user.Email = model.Email;
            user.FullName = model.FullName;
            user.DateOfBirth = model.BirthDate;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.EncryptedPassword = _passwordEncryptor.Encrypt(model.NewPassword);
            }

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);

            _context.SaveChanges();
            ViewBag.Success = "Profil başarıyla güncellendi.";
            return View(model);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
