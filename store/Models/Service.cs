using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 

namespace store.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Servis adı gereklidir")]
        [Display(Name = "Servis Adı")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Display(Name = "Fiyat")]
        [Range(0, double.MaxValue, ErrorMessage = "Fiyat 0'dan büyük olmalıdır")]
        [Column(TypeName = "decimal(10,2)")] 
        public decimal Price { get; set; }

        [Display(Name = "Süre (Dakika)")]
        [Range(1, int.MaxValue, ErrorMessage = "Süre 1 dakikadan büyük olmalıdır")]
        public int Duration { get; set; } = 60;

        [Display(Name = "Aktif")]
        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
