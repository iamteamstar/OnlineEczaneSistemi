using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class PharmacyRegistrationRequest
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string PharmacyName { get; set; }

        [Required, MaxLength(150)]
        [EmailAddress]
        public string Email { get; set; }

        [Required, MaxLength(300)]
        public string Address { get; set; }

        [Required, MaxLength(20)]
        public string Phone { get; set; }

        [Required, MaxLength(20)]
        public string TaxNumber { get; set; } // Vergi No

        [Required, MinLength(6)]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password", ErrorMessage = "Parolalar uyuşmuyor.")]
        public string ConfirmPassword { get; set; }

        // Pending | Approved | Rejected
        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
