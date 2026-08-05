using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace store.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Müşteri gereklidir")]
        [Display(Name = "Müşteri")]
        public int CustomerId { get; set; }
        
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; } = null!;
        
        [Required(ErrorMessage = "Servis gereklidir")]
        [Display(Name = "Servis")]
        public int ServiceId { get; set; }
        
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; } = null!;
        
        [Display(Name = "Personel")]
        public int? StaffId { get; set; }
        
        [ForeignKey("StaffId")]
        public virtual Staff? Staff { get; set; }
        
        [Required(ErrorMessage = "Randevu tarihi gereklidir")]
        [Display(Name = "Randevu Tarihi")]
        [DataType(DataType.DateTime)]
        public DateTime AppointmentDate { get; set; }
        
        [Display(Name = "Durum")]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
        
        [Display(Name = "Notlar")]
        public string? Notes { get; set; }
        
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
    
    public enum AppointmentStatus
    {
        [Display(Name = "Beklemede")]
        Pending = 0,
        [Display(Name = "Onaylandı")]
        Confirmed = 1,
        [Display(Name = "Tamamlandı")]
        Completed = 2,
        [Display(Name = "İptal Edildi")]
        Cancelled = 3
    }
}

