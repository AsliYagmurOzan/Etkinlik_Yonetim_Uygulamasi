using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace EtkinlikYonetim.Application.Services.Security
{
    public class PasswordEncryptor
    {
        private readonly string _secretKey;

        public PasswordEncryptor(IConfiguration configuration)
        {
            _secretKey = configuration["Encryption:SecretKey"]
                ?? throw new InvalidOperationException("Encryption:SecretKey appsettings.json'da bulunamadı.");
        }

        private Aes CreateAes()
        {
            var aes = Aes.Create();
            var keyBytes = Encoding.UTF8.GetBytes(_secretKey.PadRight(32));
            aes.Key = keyBytes[..32];
            aes.IV = new byte[16];
            return aes;
        }

        public string Encrypt(string plainText)
        {
            using var aes = CreateAes();
            using var enc = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, enc, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
                sw.Write(plainText);
            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string encryptedText)
        {
            using var aes = CreateAes();
            using var dec = aes.CreateDecryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream(Convert.FromBase64String(encryptedText));
            using var cs = new CryptoStream(ms, dec, CryptoStreamMode.Read);
            using var sr = new StreamReader(cs);
            return sr.ReadToEnd();
        }
    }
}
