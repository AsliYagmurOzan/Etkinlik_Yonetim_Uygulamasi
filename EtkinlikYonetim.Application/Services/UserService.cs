using EtkinlikYonetim.Application.Interfaces.Repositories;
using EtkinlikYonetim.Application.Services.Security;
using EtkinlikYonetim.Domain.Entities;

namespace EtkinlikYonetim.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordEncryptor _encryptor;

        public UserService(IUserRepository userRepository, PasswordEncryptor encryptor)
        {
            _userRepository = userRepository;
            _encryptor = encryptor;
        }

        public bool Register(User user, string plainPassword)
        {
            if (_userRepository.GetByEmail(user.Email) != null)
                return false;

            user.EncryptedPassword = _encryptor.Encrypt(plainPassword);
            _userRepository.Add(user);
            return true;
        }

        public bool Login(string email, string plainPassword)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null) return false;

            string decryptedPassword;
            try
            {
                decryptedPassword = _encryptor.Decrypt(user.EncryptedPassword);
            }
            catch
            {
                return false;
            }

            return plainPassword == decryptedPassword;
        }
    }
}
