using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace EtkinlikYonetim.Models
{
    public class EventFormViewModel
    {
        public int? Id { get; set; }

        [Display(Name = "Etkinlik Başlığı")]
        [Required(ErrorMessage = "Etkinlik başlığı gereklidir.")]
        [MaxLength(255, ErrorMessage = "Etkinlik başlığı en fazla 255 karakter olabilir.")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Başlangıç Tarihi")]
        [Required(ErrorMessage = "Başlangıç tarihi gereklidir.")]
        [DataType(DataType.DateTime, ErrorMessage = "Geçerli bir tarih ve saat giriniz.")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Bitiş Tarihi")]
        [Required(ErrorMessage = "Bitiş tarihi gereklidir.")]
        [DataType(DataType.DateTime, ErrorMessage = "Geçerli bir tarih ve saat giriniz.")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Etkinlik Durumu")]
        [Required(ErrorMessage = "Etkinlik durumu seçilmelidir.")]
        public bool IsActive { get; set; }

        [Display(Name = "Kısa Açıklama")]
        [Required(ErrorMessage = "Kısa açıklama gereklidir.")]
        [MaxLength(512, ErrorMessage = "Kısa açıklama en fazla 512 karakter olabilir.")]
        public string ShortDescription { get; set; } = string.Empty;

        [Display(Name = "Uzun Açıklama")]
        [ValidateNever] 
        public string? LongDescription { get; set; }


        [Display(Name = "Etkinlik Resmi")]
        public IFormFile? ImageFile { get; set; }

        [ValidateNever]
        public string? ExistingImagePath { get; set; }
    }
}
