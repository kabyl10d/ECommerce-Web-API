using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleCrud.Data.Entities
{
    public class Customer
    {

        [Key]
        public Guid CustomerId { get; set; }
        public required string CustomerName { get; set; }

        public ICollection<Order> Orders { get; } = new List<Order>();
        public ICollection<CartItem> Cart { get; } = new List<CartItem>();

    }
}
