using Apple.Services.AuthAPI.Data;
using Apple.Services.AuthAPI.Models;
using Apple.Services.AuthAPI.Models.Dto;
using Apple.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Apple.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        public readonly AppDbContext _dbContext;
        public readonly UserManager<ApplicationUser> _userManager;
        public readonly RoleManager<IdentityRole> _roleManager;
        public readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext appDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IJwtTokenGenerator jwtTokenGenerator)
        {
            _dbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Register(RegisterDto registerDto)
        {
            ApplicationUser appUser = new ApplicationUser()
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                NormalizedEmail = registerDto.Email.ToUpper(),
                Name = registerDto.Name,
                PhoneNumber = registerDto.PhoneNumber,
            };

            try
            {
                var result = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (result.Succeeded)
                {
                    var userFound = _dbContext.ApplicationUsers.First(f => f.UserName == registerDto.Email);
                    UserDto userDto = new UserDto()
                    {
                        Email = userFound.Email,
                        ID = userFound.Id,
                        Name = userFound.Name,
                        PhoneNumber = userFound.PhoneNumber
                    };
                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {
            }
            return "Error encountered";
        }

        public async Task<LoginResponseDto> Login(LoginDto loginDto)
        {
            var userFound = _dbContext.ApplicationUsers.FirstOrDefault(f => f.UserName.ToLower() == loginDto.UserName.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(userFound, loginDto.Password);
            if(userFound is null ||  !isValid)
            {
                return new LoginResponseDto() { User=null,Token="" };
            }

            UserDto userDto = new UserDto()
            {
                Email = userFound.Email,
                ID = userFound.Id,
                Name = userFound.Name,
                PhoneNumber = userFound.PhoneNumber
            };

            var roles = await _userManager.GetRolesAsync(userFound);

            var token = _jwtTokenGenerator.GenerateToken(userFound, roles);
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDto,
                Token = token
            };

            return loginResponseDto;

        }

        public async Task<bool> AssignRole(string email, string role)
        {
            var userFound = _dbContext.ApplicationUsers.FirstOrDefault(f => f.Email.ToLower() == email.ToLower());
            if (userFound != null)
            {
                if(!_roleManager.RoleExistsAsync(role).GetAwaiter().GetResult())
                {
                    _roleManager.CreateAsync(new IdentityRole(role)).GetAwaiter().GetResult() ;
                }

                await _userManager.AddToRoleAsync(userFound, role);
                return true;
            }
            return false;
        }
    }
}