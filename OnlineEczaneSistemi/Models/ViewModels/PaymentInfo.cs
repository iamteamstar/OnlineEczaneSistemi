using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class PaymentInfo
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        public string Method { get; set; } // Cash / CreditCard / Online

        public bool IsPaid { get; set; } = false;

        public decimal Amount { get; set; }
    }
}
