using System.ComponentModel.DataAnnotations;

namespace OnlineEczaneSistemi.Models.ViewModels
{
    public class ProfilEdit
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        // Eski profil resmi (sadece gösterim için)
        [Display(Name = "Mevcut Profil Resmi")]
        public string? ExistingProfileImagePath { get; set; }

        // Yeni profil fotoğrafı (kullanıcı isterse yükler)
        [Display(Name = "Yeni Profil Resmi")]
        public IFormFile? NewProfileImage { get; set; }
    }
}
