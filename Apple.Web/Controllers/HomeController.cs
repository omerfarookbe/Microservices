using Apple.Web.Models;
using Apple.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using IdentityModel;

namespace Apple.Web.Controllers
{
    public class HomeController : Controller
    {
        private IProductService _productService;
        public ICartService _cartService { get; }

        public HomeController(IProductService productService, ICartService cartService)
        {
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            List<ProductDto?> result = new();
            ResponseDto? response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSuccess)
            {
                result = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(result);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? result = new();
            ResponseDto? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                result = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(result);
        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(f => f.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };

            List<CartDetailsDto> cartDetailsDtos = new() 
            {
                cartDetails 
            };

            cartDto.CartDetails = cartDetailsDtos;

            ProductDto? result = new();
            ResponseDto? response = await _cartService.UpsertCartAsync(cartDto);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to the cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDto);

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
