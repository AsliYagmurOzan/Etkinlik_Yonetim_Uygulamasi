using EtkinlikYonetim.Application.Interfaces.Repositories;
using EtkinlikYonetim.Domain.Entities;
using EtkinlikYonetim.Infrastructure.Context;

namespace EtkinlikYonetim.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EventDbContext _context;

        public UserRepository(EventDbContext context)
        {
            _context = context;
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public User? GetByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
