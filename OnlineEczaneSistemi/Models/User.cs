using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]//id otomatik artsın 
        public int UserId { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; }
        [Required, MaxLength(150)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string? ImageUrl { get; set; }//resimler wwwroot altında tutulacak ve db'de uzantı olarak kaydedilecek
        [Required]
        public string Role { get; set; } = "User";//varsayılan kullanıcı user
        public DateTime CreatedAt { get; set; } = DateTime.Now;//admin kullanıcı kayıt tarihini görecek
        public bool IsActive { get; set; } = true;//admin, kullanıcı aktif/pasif yapabilecek
    }
}
