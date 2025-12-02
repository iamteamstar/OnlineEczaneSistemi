using System.ComponentModel.DataAnnotations;

namespace OnlineEczaneSistemi.Models.ViewModels
{
    public class Register
    {
        [Required, MaxLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; }
        [Required, MaxLength(150)]
        [EmailAddress(ErrorMessage = "Lütfen geçerli e-mail adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required, MinLength(8, ErrorMessage = "Parola en az 8 hane olmalı")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Parolayı tekrar giriniz")]
        [DataType(DataType.Password)]
        [Display(Name = "Parola Tekrar")]
        [Compare("Password", ErrorMessage = "Parolalar uyuşmuyor")]
        public string PasswordApprove { get; set; }//parola tekrarı
        [Display(Name = "Profil Resmi")]

        public IFormFile? ProfileImage { get; set; }
    }
}
