using Apple.Services.CouponAPI.Data;
using Apple.Services.CouponAPI.Models;
using Apple.Services.CouponAPI.Models.Dto;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Apple.Services.CouponAPI.Controller
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private IMapper _mapper;

        private ResponseDto _response;

        public CouponAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _dbContext = appDbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }


        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Coupon> result = _dbContext.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(result);
            }
            catch (Exception ex)
            {
                _response.IsSuccess=false;
                _response.Message= ex.Message;  
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon result = _dbContext.Coupons.First(f => f.CouponId == id);                
                _response.Result = _mapper.Map<CouponDto>(result); ;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon result = _dbContext.Coupons.First(f => f.CouponCode.ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDto>(result); ;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _dbContext.Coupons.Add(coupon);
                _dbContext.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(couponDto); ;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _dbContext.Coupons.Update(coupon);
                _dbContext.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(couponDto); ;
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int Id)
        {
            try
            {
                Coupon coupon = _dbContext.Coupons.First(f => f.CouponId == Id);
                _dbContext.Remove(coupon);
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
