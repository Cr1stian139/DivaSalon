using System.ComponentModel.DataAnnotations;

namespace DivaSalon.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        [Range(0, 100000)]
        public decimal PriceFrom { get; set; }

        [Range(0, 100000)]
        public decimal? PriceTo { get; set; }  // Acum este optional (nullable)

        public bool IsActive { get; set; } = true;
    }
}