using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace EtkinlikYonetim.Models
{
    public class EventFormViewModel
    {
        public int? Id { get; set; } 

        [Required(ErrorMessage = "Etkinlik başlığı gereklidir.")]
        [MaxLength(255, ErrorMessage = "Etkinlik başlığı en fazla 255 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlangıç tarihi gereklidir.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş tarihi gereklidir.")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Etkinlik durumu seçilmelidir.")]
        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Kısa açıklama gereklidir.")]
        [MaxLength(512, ErrorMessage = "Kısa açıklama en fazla 512 karakter olabilir.")]
        public string ShortDescription { get; set; } = string.Empty;

        public string? LongDescription { get; set; }

        public IFormFile? ImageFile { get; set; }

        public string? ExistingImagePath { get; set; }
    }
}
