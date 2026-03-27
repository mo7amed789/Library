using System.ComponentModel.DataAnnotations;

namespace Simple_LBApi.DTOs
{
    public class RegisterDto
    {
        [Required, MaxLength(80)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(35)]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(12), MaxLength(128)]
        public string Password { get; set; } = string.Empty;
    }
}
