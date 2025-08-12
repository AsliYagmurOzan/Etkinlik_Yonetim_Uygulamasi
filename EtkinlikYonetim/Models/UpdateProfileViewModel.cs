using System;
using System.ComponentModel.DataAnnotations;

namespace EtkinlikYonetim.Models
{
    public class UpdateProfileViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required] 
        public string CurrentPassword { get; set; } = string.Empty;

        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Yeni parola en az bir büyük harf, bir küçük harf ve bir rakam içermelidir.")]
        public string? NewPassword { get; set; }

        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required, DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }

}
