using System.ComponentModel.DataAnnotations;

namespace store.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Ad gereklidir")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Soyad gereklidir")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Telefon gereklidir")]
        [Display(Name = "Telefon")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
        public string Phone { get; set; } = string.Empty;
        
        [Display(Name = "E-posta")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        public string? Email { get; set; }
        
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Ad Soyad")]
        public string FullName => $"{FirstName} {LastName}";
        
        // Navigation property
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}

