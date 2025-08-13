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

        public UserController(EventDbContext context, PasswordEncryptor passwordEncryptor)
        {
            _context = context;
            _passwordEncryptor = passwordEncryptor;
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(UserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.DateOfBirth == null)
            {
                ModelState.AddModelError(nameof(UserViewModel.DateOfBirth), "Doğum tarihi zorunludur.");
                return View(model);
            }

            if (model.DateOfBirth > DateTime.Today)
            {
                ModelState.AddModelError(nameof(UserViewModel.DateOfBirth), "Doğum tarihi bugünden ileri bir tarih olamaz.");
                return View(model);
            }

            var email = model.Email?.Trim() ?? string.Empty;
            if (_context.Users.Any(u => u.Email == email))
            {
                ModelState.AddModelError(nameof(UserViewModel.Email), "Bu e-posta adresi zaten kayıtlı.");
                return View(model);
            }

            var encryptedPassword = _passwordEncryptor.Encrypt(model.Password);

            var user = new User
            {
                FullName = model.FullName?.Trim(),
                Email = email,
                EncryptedPassword = encryptedPassword,
                DateOfBirth = model.DateOfBirth.Value,
                CreatedAt = DateTime.Now
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var email = model.Email?.Trim();

            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError(nameof(LoginViewModel.Email),
                    "Bu e-posta adresi ile kayıtlı bir kullanıcı bulunamadı.");
                return View(model);
            }

            string decryptedPassword;
            try
            {
                decryptedPassword = _passwordEncryptor.Decrypt(user.EncryptedPassword);
            }
            catch
            {
                ModelState.AddModelError(nameof(LoginViewModel.Password),
                    "Parola doğrulama sırasında bir hata oluştu.");
                return View(model);
            }

            if (!string.Equals(model.Password, decryptedPassword))
            {
                ModelState.AddModelError(nameof(LoginViewModel.Password),
                    "Parola hatalı. Lütfen tekrar deneyin.");
                return View(model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);
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
        [ValidateAntiForgeryToken]
        public IActionResult Profile(UpdateProfileViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return RedirectToAction("Login");

            var currentDecrypted = _passwordEncryptor.Decrypt(user.EncryptedPassword);
            if (model.CurrentPassword != currentDecrypted)
            {
                ModelState.AddModelError("CurrentPassword", "Mevcut parola yanlış.");
                return View(model);
            }

            if (model.Email != user.Email && _context.Users.Any(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Bu mail adresi başka bir kullanıcıya ait.");
                return View(model);
            }

            user.Email = model.Email;
            user.FullName = model.FullName;
            user.DateOfBirth = model.BirthDate;

            if (!string.IsNullOrWhiteSpace(model.NewPassword))
                user.EncryptedPassword = _passwordEncryptor.Encrypt(model.NewPassword);

            _context.SaveChanges();

            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);

            ViewBag.Success = "Profil başarıyla güncellendi.";
            return View(model);
        }

        public IActionResult AccessDenied() => View();

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
