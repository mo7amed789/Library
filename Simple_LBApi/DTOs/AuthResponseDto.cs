namespace Simple_LBApi.DTOs
{
    public sealed class AuthResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
