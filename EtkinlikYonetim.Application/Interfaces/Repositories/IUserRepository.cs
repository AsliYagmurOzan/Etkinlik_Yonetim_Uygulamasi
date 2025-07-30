using EtkinlikYonetim.Domain.Entities;

namespace EtkinlikYonetim.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        void Add(User user);
        User? GetByEmail(string email);
    }
}
