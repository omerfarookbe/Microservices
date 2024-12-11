using Apple.Web.Models;

namespace Apple.Web.Service.IService
{
    public interface IAuthService
    {
        public Task<ResponseDto> LoginAsync(LoginDto loginDto);
        public Task<ResponseDto> RegisterAsync(RegisterDto registerDto);
        public Task<ResponseDto> AssignRoleAsync(RegisterDto registerDto);
    }
}
