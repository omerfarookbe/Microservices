using System.ComponentModel.DataAnnotations;

namespace Apple.Web.Models
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }

        [Required] 
        public string Name { get; set; }

        [Required] 
        public string PhoneNumber { get; set; }

        [Required] 
        public string Password { get; set; }

        public string? Role { get; set; }
    }
}
