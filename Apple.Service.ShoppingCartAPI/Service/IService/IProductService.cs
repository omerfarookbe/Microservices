using Apple.Services.ShoppingCartAPI.Models.Dtos;

namespace Apple.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        public Task<IEnumerable<ProductDto>> GetProductsAsync();
    }
}
