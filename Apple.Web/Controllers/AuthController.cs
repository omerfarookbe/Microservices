using Apple.Web.Models;
using Apple.Web.Service.IService;
using Apple.Web.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Apple.Web.Controllers
{
    public class AuthController : Controller
    {
        public readonly IAuthService _authService;

        public readonly ITokenProvider _tokenProvider;

        public AuthController(IAuthService authService, ITokenProvider tokenProvider)
        {
            _authService = authService;
            _tokenProvider = tokenProvider;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var role = new List<SelectListItem>()
            {
                new SelectListItem{ Text=SD.RoleAdmin, Value=SD.RoleAdmin },
                new SelectListItem{ Text=SD.RoleCustomer, Value=SD.RoleCustomer },
            };

            ViewBag.Role = role;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            ResponseDto response = await _authService.RegisterAsync(registerDto);
            ResponseDto assignRole;
            if (response != null && response.IsSuccess)
            {
                if (string.IsNullOrEmpty(registerDto.Role))
                {
                    registerDto.Role = SD.RoleCustomer;
                }
                assignRole = await _authService.AssignRoleAsync(registerDto);
                if (assignRole != null && assignRole.IsSuccess)
                {
                    TempData["success"] = "Registration successful";
                    return RedirectToAction(nameof(Login));
                }
            }
            else
            {
                TempData["error"] = response.Message;
            }

            var role = new List<SelectListItem>()
            {
                new SelectListItem{ Text=SD.RoleAdmin, Value=SD.RoleAdmin },
                new SelectListItem{ Text=SD.RoleCustomer, Value=SD.RoleCustomer },
            };

            ViewBag.Role = role;
            return View(registerDto);
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginDto loginDto = new LoginDto();
            return View(loginDto);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            ResponseDto response = await _authService.LoginAsync(loginDto);
            if (response != null && response.IsSuccess)
            {
                LoginResponseDto loginResponseDto = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                await SignInUser(loginResponseDto);
                _tokenProvider.SetToken(loginResponseDto.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["error"] = response.Message;
                return View(loginDto);
            }
        }
        
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            _tokenProvider.ClearedToken();
            return RedirectToAction("Index","Home");
        }

        private async Task SignInUser(LoginResponseDto loginResponseDto)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(loginResponseDto.Token);

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Email,jwt.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Email).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub,jwt.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Sub).Value));
            identity.AddClaim(new Claim(JwtRegisteredClaimNames.Name,jwt.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Name).Value));

            identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(f => f.Type == JwtRegisteredClaimNames.Email).Value));

            identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(f => f.Type == "role").Value));


            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}
