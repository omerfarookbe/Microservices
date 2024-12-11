using Apple.Services.ProductAPI.Models;
using AutoMapper;
using Apple.Services.ProductAPI.Models;
using Apple.Services.ProductAPI.Models.Dto;
namespace Mango.Services.ProductAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
            });
            return mappingConfig;
        }
    }
}