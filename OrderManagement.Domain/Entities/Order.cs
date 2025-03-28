using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagement.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string Cliente { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // Relación con OrderProducts
        public List<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}
