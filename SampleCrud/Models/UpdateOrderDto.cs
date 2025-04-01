using static SampleCrud.Common.Enums;

namespace SampleCrud.Models
{
    public class UpdateOrderDto
    {
        public required OrderStatus Status { get; set; }
    }
}
