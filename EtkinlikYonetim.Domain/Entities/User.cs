namespace EtkinlikYonetim.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // Şifrenin hashli hali
        public string PasswordHash { get; set; } = string.Empty;        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Event>? CreatedEvents { get; set; }
    }
}
