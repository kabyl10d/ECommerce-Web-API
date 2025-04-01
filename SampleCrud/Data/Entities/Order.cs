using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Data.Entities
{
    public class Order
    {
        public int OrderId { get; set; }

         public Guid CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;


        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public double TotalAmount { get; set; } 
        public required PaymentMode PaymentMethod { get; set; } 
        public DateTime OrderDate { get; set; } 

        [MaxLength(50)]
        public required string ShippingAddress { get; set; }

        public required OrderStatus Status { get; set; }
 
    }   
}
