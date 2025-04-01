using System.ComponentModel.DataAnnotations;

namespace SampleCrud.Data.Entities
{
    public class User
    {
        public Guid UserId { get; set; }

        [MaxLength(20)]
        public string Username { get; set; }

        public string Mailid { get; set; }

        [MinLength(8)]
        public string Password { get; set; }

        [MaxLength(10)]  
        public string Role { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

       
    }

  
}
