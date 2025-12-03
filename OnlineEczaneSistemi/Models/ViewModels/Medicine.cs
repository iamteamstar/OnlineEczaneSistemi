using System.ComponentModel.DataAnnotations;

namespace OnlineEczaneSistemi.Models
{
    public class Medicine
    {
        [Key]
        public int MedicineId { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public int Stock { get; set; }

        [Required]
        public decimal Price { get; set; }

        // İlacın sahibi olan eczane
        public int PharmacyId { get; set; }
    }
}
