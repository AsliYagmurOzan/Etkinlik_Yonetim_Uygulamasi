using System;
using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetim.Models
{
    public class UpdateProfileViewModel
    {
        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Mevcut Parola")]
        [Required(ErrorMessage = "Mevcut parola alanı zorunludur.")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Display(Name = "Yeni Parola")]
        [MinLength(8, ErrorMessage = "Yeni parola en az 8 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Yeni parola en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string? NewPassword { get; set; }

        [Display(Name = "Ad Soyad")]
        [Required(ErrorMessage = "Ad Soyad alanı zorunludur.")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Doğum Tarihi")]
        [Required(ErrorMessage = "Doğum tarihi alanı zorunludur.")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}
