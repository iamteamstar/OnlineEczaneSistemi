using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        // Hasta (User)
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }

        // Eczane
        public int? PharmacyId { get; set; }

        [ForeignKey("PharmacyId")]
        public User Pharmacy { get; set; }

        // Kurye
        public int? CourierId { get; set; }

        [ForeignKey("CourierId")]
        public User Courier { get; set; }

        // Sipariş toplam fiyat
        public decimal TotalPrice { get; set; }

        // "Pending", "Preparing", "WaitingCourier", "OnTheWay", "Delivered", "Cancelled"
        [Required]
        public string Status { get; set; } = "Pending";

        // Teslimat adresi
        [Required, MaxLength(300)]
        public string DeliveryAddress { get; set; }// teslimat adresi

        public string PrescriptionUrl { get; set; }  // yüklenen dosyanın yolu
        public string Notes { get; set; }            // hastanın yazdığı açıklama

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; }
        public List<OrderStatusHistory> History { get; set; }
        public PaymentInfo Payment { get; set; }
    }
}
