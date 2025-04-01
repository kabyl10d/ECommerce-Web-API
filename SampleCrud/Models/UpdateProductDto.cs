namespace SampleCrud.Models
{
    public class UpdateProductDto
    {
        public required string Name { get; set; }
        public required double Price { get; set; }

        public required int Stock { get; set; }

        public required Guid MerchId { get; set; }

        public required string Category { get; set; }

    }
}
