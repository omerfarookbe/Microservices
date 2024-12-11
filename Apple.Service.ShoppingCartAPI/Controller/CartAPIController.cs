using AutoMapper;
using Apple.Services.ShoppingCartAPI.Data;
using Apple.Services.ShoppingCartAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Apple.Services.ShoppingCartAPI.Models.Dtos;
using System.Collections.Generic;
using Apple.Services.ShoppingCartAPI.Service.IService;
using Apple.MessageBus;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly AppDbContext _dbContext;
        private readonly IMessageBus _messageBus;

        public IConfiguration _configuration;

        public CartAPIController(AppDbContext dbContext, IMapper mapper, IProductService productService, ICouponService couponService, IMessageBus messageBus, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _response = new ResponseDto();
            _mapper = mapper;
            _productService = productService;
            _couponService = couponService;
            _messageBus = messageBus;
            _configuration = configuration;
        }

        [HttpGet]
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_dbContext.CartHeaders.First(f => f.UserId == userId))
                };
                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(_dbContext.CartDetails.Where(f => f.CartHeaderId == cart.CartHeader.CartHeaderId));

                IEnumerable<ProductDto> productDtos = await _productService.GetProductsAsync();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(f => f.ProductId == item.ProductId);
                    cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon
                if(!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    CouponDto couponDto = await _couponService.GetCouponAsync(cart.CartHeader.CouponCode);
                    if(couponDto != null && cart.CartHeader.CartTotal > couponDto.MinAmount) 
                    { 
                        cart.CartHeader.CartTotal -= couponDto.DiscountAmount;
                        cart.CartHeader.Discount = couponDto.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cart = await _dbContext.CartHeaders.FirstAsync(f => f.UserId == cartDto.CartHeader.UserId);
                cart.CouponCode = cartDto.CartHeader.CouponCode;
                _dbContext.Update(cart);
                await _dbContext.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        //[HttpPost("RemoveCoupon")]
        //public async Task<ResponseDto> RemoveCoupon([FromBody] CartDto cartDto)
        //{
        //    try
        //    {
        //        var cart = await _dbContext.CartHeaders.FirstAsync(f => f.UserId == cartDto.CartHeader.UserId);
        //        cart.CouponCode = "";
        //        _dbContext.Update(cart);
        //        await _dbContext.SaveChangesAsync();
        //        _response.Result = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _response.Message = ex.Message.ToString();
        //        _response.IsSuccess = false;
        //    }
        //    return _response;
        //}

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _dbContext.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _dbContext.CartHeaders.Add(cartHeader);
                    await _dbContext.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _dbContext.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _dbContext.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _dbContext.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _dbContext.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                var cartDetails = _dbContext.CartDetails.First(u => u.CartDetailsId == cartDetailsId);
                _dbContext.CartDetails.Remove(cartDetails);

                int totalCountOfCartItems = _dbContext.CartDetails.Where(f => f.CartHeaderId == cartDetails.CartHeaderId).Count();
                if (totalCountOfCartItems == 1)
                {
                    var cartHeaderToRemove = await _dbContext.CartHeaders.FirstOrDefaultAsync(f => f.CartHeaderId == cartDetails.CartHeaderId);
                    _dbContext.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _dbContext.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("EmailCart")]
        public async Task<ResponseDto> EmailCart([FromBody] CartDto cartDto)
        {
            try
            {
                await _messageBus.PublishMessage(cartDto, _configuration.GetValue<string>("ServiceBus:QueueName"));
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

    }
}