using System.ComponentModel.DataAnnotations;

namespace OnlineEczaneSistemi.Models.ViewModels
{
    public class Login
    {
        [Required]
        [EmailAddress]
        [Display(Name = "E-Posta")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
