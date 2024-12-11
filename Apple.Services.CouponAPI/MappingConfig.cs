using Apple.Services.CouponAPI.Models;
using AutoMapper;

namespace Apple.Services.CouponAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingconfg = new MapperConfiguration(config => { 
                config.CreateMap<CouponDto, Coupon>();
                config.CreateMap<Coupon, CouponDto>();
            });
            return mappingconfg;
        }
    }
}
