using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Data.Entities
{

    public class Product
    {
        public Guid ProductId { get; set; }

        //public ICollection<Order> Orders { get; } = new List<Order>();
        //public ICollection<CartItem> CartItems { get; } = new List<CartItem>();


        [MaxLength(50)]
        public required string Name { get; set; }
        public required double Price { get; set; }
        public required int Stock { get; set; }

         //public required string MerchName { get; set; }
 
         public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
        public required Category Category { get; set; }

      
    }
}
