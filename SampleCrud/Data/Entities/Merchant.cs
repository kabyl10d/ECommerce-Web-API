using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SampleCrud.Data.Entities
{
    public class Merchant 
    {

        [Key]
        public Guid MerchantId { get; set; }

        public required string MerchantName { get; set; }

        public ICollection<Product> Products { get; } = new List<Product>();
    }
}