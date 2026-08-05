using System.ComponentModel.DataAnnotations;

namespace store.Models
{
    public class AdminUser
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Kullanıcı adı gereklidir")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Şifre gereklidir")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        
        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }
        
        [Display(Name = "Ad")]
        public string? FirstName { get; set; }
        
        [Display(Name = "Soyad")]
        public string? LastName { get; set; }
        
        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}

