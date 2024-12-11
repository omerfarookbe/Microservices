using Apple.Web.Models;

namespace Apple.Web.Service.IService
{
    public interface ICartService
    {
        public Task<ResponseDto?> GetCartByUserIdAsync(string userId);

        public Task<ResponseDto?> UpsertCartAsync(CartDto cartDto);

        public Task<ResponseDto?> RemoveCartAsync(int cartDetailsId);

        public Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto);

        public Task<ResponseDto?> EmailCart(CartDto cartDto);
    }
}
