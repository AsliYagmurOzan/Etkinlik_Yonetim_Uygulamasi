using System;
using System.Security.Cryptography;
using System.Text;
namespace EtkinlikYonetim.Application.Services.Security
{
    public class PasswordHasher
    {
        private readonly string _secretKey = "Sifre";

        public string HashPassword(string password)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hash);
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }
}