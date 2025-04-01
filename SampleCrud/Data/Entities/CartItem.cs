using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Data.Entities
{
    public class CartItem
    {
        [Key]
        public int ItemId { get; set; }

    
        public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

         
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;


        public int Quantity { get; set; }
 
        public OrderStatus Status { get; set; }

    }
}
