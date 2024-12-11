using Apple.Web.Models;

namespace Apple.Web.Service.IService
{
    public interface ICouponService
    {
        public Task<ResponseDto?> GetCouponAsync(string couponCode);

        public Task<ResponseDto?> GetAllCouponAsync();

        public Task<ResponseDto?> GetCouponByIdAsync(int id);

        public Task<ResponseDto?> CreateCouponAsync(CouponDto couponDto);

        public Task<ResponseDto?> UpdateCouponAsync(CouponDto couponDto);

        public Task<ResponseDto?> DeleteCouponAsync(int id);
    }
}
