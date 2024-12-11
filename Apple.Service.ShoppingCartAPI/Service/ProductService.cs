using Apple.Services.ShoppingCartAPI.Models.Dtos;
using Apple.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Apple.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<IEnumerable<ProductDto>> GetProductsAsync()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (result.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(result.Result));
            }
            return new List<ProductDto>();
        }
    }
}
