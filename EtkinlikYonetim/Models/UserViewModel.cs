using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetim.Models
{
    public class UserViewModel
    {
        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Parola alanı zorunludur.")]
        [MinLength(8, ErrorMessage = "Parola en az 8 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Parola en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Parola Tekrar")]
        [Required(ErrorMessage = "Parola tekrarı zorunludur.")]
        [Compare("Password", ErrorMessage = "Parolalar eşleşmiyor.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Doğum Tarihi")]
        [Required(ErrorMessage = "Doğum tarihi alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }
}
