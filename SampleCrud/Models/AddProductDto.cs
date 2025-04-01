namespace SampleCrud.Models
{
    public class AddProductDto
    {
     
        public required string Name { get; set; }
        public required double Price { get; set; }

        public required int Stock { get; set; }

        //public required Guid MerchId { get; set; }

        //public required string MerchName { get; set; }

        public required string Category { get; set; }
    
    }
}
