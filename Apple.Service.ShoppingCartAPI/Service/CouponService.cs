using Apple.Services.ShoppingCartAPI.Models.Dtos;
using Apple.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Apple.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<CouponDto> GetCouponAsync(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto> (Convert.ToString(result.Result));
            }
            return new CouponDto();
        }
    }
}
