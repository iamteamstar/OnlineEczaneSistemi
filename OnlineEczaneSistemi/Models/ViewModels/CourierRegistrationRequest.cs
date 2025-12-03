using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class CourierRegistrationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [Required, MaxLength(50)]
        public string VehicleType { get; set; } // Motor, Araba, Bisiklet

        [Required, MinLength(6)]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Parolalar uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        // Pending | Approved | Rejected
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
