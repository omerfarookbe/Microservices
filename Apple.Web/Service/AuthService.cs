using Apple.Web.Models;
using Apple.Web.Service.IService;
using Apple.Web.Utility;

namespace Apple.Web.Service
{
    public class AuthService : IAuthService
    {
        public readonly IBaseService _baseService;

        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegisterDto registerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerDto,
                Url = SD.AuthAPIBase + "/api/auth/assignrole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginDto loginDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = registerDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, false);
        }
    }
}
