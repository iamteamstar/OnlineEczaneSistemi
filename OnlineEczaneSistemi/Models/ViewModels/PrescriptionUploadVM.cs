using System.ComponentModel.DataAnnotations;

namespace OnlineEczaneSistemi.Models.ViewModels
{
    public class PrescriptionUploadVM
    {
        [Required(ErrorMessage = "Lütfen bir reçete dosyası yükleyin.")]
        public IFormFile PrescriptionFile { get; set; }

        public string Notes { get; set; }

        [Required(ErrorMessage = "Adres zorunludur.")]
        public string DeliveryAddress { get; set; }

        [Required(ErrorMessage = "Lütfen bir eczane seçin.")]
        public int? PharmacyId { get; set; }

        public List<User> Pharmacies { get; set; }
    }

}
