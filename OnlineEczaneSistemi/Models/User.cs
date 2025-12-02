using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Otomatik artan ID
        public int UserId { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Profil fotoğrafının URL yolu
        public string? ImageUrl { get; set; }

        // Roller: User, Admin, Pharmacy, Courier
        [Required]
        public string Role { get; set; } = "User";

        // Kullanıcı oluşturulma tarihi
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Hesap aktif/pasif durumu
        public bool IsActive { get; set; } = true;
    }
}
