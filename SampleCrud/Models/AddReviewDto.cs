using static SampleCrud.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace SampleCrud.Models
{
    public class AddReviewDto
    {
          
        public string ReviewText { get; set; }

        [Required]
        public ReviewType ReviewType { get; set; }
    }
}
