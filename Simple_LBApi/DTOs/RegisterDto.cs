using System.ComponentModel.DataAnnotations;

namespace Simple_LBApi.DTOs
{
    public class RegisterDto
    {
        public string Name { get; set; }
        [Required, EmailAddress, MaxLength(35)]
        public string Email { get; set; }
        [Required, MinLength(12), MaxLength(128)]
        public string Password { get; set; }
    }
}
