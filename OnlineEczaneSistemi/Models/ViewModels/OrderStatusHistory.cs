using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineEczaneSistemi.Models
{
    public class OrderStatusHistory
    {
        [Key]
        public int HistoryId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        // Status: Pending, Preparing, WaitingCourier, OnTheWay, Delivered
        [Required]
        public string Status { get; set; }

        public DateTime ChangedAt { get; set; } = DateTime.Now;
    }
}
