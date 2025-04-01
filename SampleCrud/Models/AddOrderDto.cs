using static SampleCrud.Common.Enums;

namespace SampleCrud.Models
{
    public class AddOrderDto
    { 
        public required PaymentMode PaymentMethod { get; set; } 
        public required string ShippingAddress { get; set; }

    }
}
