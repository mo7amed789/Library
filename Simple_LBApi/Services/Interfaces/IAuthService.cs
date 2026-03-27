using Simple_LBApi.DTOs;

namespace Simple_LBApi.Services.Interfaces
{
    public interface IAuthService
    {

        Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}
