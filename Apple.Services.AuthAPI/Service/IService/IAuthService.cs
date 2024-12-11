using Apple.Services.AuthAPI.Models.Dto;

namespace Apple.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        public Task<string> Register(RegisterDto registerDto);

        public Task<LoginResponseDto> Login(LoginDto loginDto);

        Task<bool> AssignRole(string email, string role);
    }
}
