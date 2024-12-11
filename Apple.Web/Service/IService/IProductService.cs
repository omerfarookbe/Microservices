using Apple.Web.Models;

namespace Apple.Web.Service.IService
{
    public interface IProductService
    {
        public Task<ResponseDto?> GetAllProductAsync();

        public Task<ResponseDto?> GetProductByIdAsync(int id);

        public Task<ResponseDto?> CreateProductAsync(ProductDto productDto);

        public Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);

        public Task<ResponseDto?> DeleteProductAsync(int id);
    }
}
