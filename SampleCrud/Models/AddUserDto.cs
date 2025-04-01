namespace SampleCrud.Models
{
    public class AddUserDto
    {
        public required string Username { get; set; }

        public required string Mailid { get; set; }
        public required string Password { get; set; }
        public required string Role { get; set; }

        public required string Phone { get; set; }
    }
}
