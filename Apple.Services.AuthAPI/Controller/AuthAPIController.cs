using Apple.Services.AuthAPI.Models.Dto;
using Apple.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace Apple.Services.AuthAPI.Controller
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        public readonly IAuthService _authService;
        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var errorMessage = await _authService.Register(registerDto);
            if(!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess=false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }

            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var loginResponse = await _authService.Login(loginDto);
            if(loginResponse.User == null) 
            { 
                _response.IsSuccess=false;
                _response.Message = "Incorrect username / password";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("assignrole")]
        public async Task<IActionResult> AssignRole([FromBody] RegisterDto registerDto)
        {
            var response = await _authService.AssignRole(registerDto.Email,registerDto.Role.ToUpper());
            if (!response)
            {
                _response.IsSuccess = false;
                _response.Message = "Errot encountered";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}