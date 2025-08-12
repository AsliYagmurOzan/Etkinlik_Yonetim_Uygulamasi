using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetim.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [DataType(DataType.Password)] 
        public string Password { get; set; } = string.Empty;
    }
}
