using static SampleCrud.Common.Enums;

namespace SampleCrud.Models
{
    public class AddCartItemDto
    {

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }

        public OrderStatus Status { get; set; }
    }
}
