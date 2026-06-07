using System.ComponentModel.DataAnnotations;

namespace DivaSalon.Models
{
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order? Order { get; set; }

        [Required]
        public int ServiceId { get; set; }

        public Service? Service { get; set; }

        public decimal Price { get; set; }
        public string ServiceName { get; set; } = string.Empty;
    }
}