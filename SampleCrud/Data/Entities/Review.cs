using System.ComponentModel.DataAnnotations;
using static SampleCrud.Common.Enums;

namespace SampleCrud.Data.Entities
{
    public class Review
    {
        public int reviewId { get; set; }

        [Required] 
        public Guid UserId { get; set; }

        [Required]
        public Guid ProductId { get; set; }

        [MaxLength(100)]
        public string ReviewText { get; set; }

        [Required]
        public ReviewType ReviewType { get; set; }
    }
}
