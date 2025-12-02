using System.ComponentModel.DataAnnotations;

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

        // Pending | Approved | Rejected
        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
