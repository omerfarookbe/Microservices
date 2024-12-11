using Apple.Services.ShoppingCartAPI.Models.Dtos;

namespace Apple.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        public Task<CouponDto> GetCouponAsync(string couponCode);
    }
}
