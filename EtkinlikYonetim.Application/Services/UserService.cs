using EtkinlikYonetim.Application.Interfaces.Repositories;
using EtkinlikYonetim.Application.Services.Security;
using EtkinlikYonetim.Domain.Entities;

namespace EtkinlikYonetim.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher _hasher;

        public UserService(IUserRepository userRepository, PasswordHasher hasher)
        {
            _userRepository = userRepository;
            _hasher = hasher;
        }

        public bool Register(User user, string plainPassword)
        {
            if (_userRepository.GetByEmail(user.Email) != null)
                return false;

            user.PasswordHash = _hasher.HashPassword(plainPassword);
            _userRepository.Add(user);
            return true;
        }

        public bool Login(string email, string plainPassword)
        {
            var user = _userRepository.GetByEmail(email);
            if (user == null) return false;

            return _hasher.VerifyPassword(plainPassword, user.PasswordHash);
        }
    }
}
