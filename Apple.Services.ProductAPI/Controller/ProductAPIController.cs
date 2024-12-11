using AutoMapper;
using Apple.Services.ProductAPI.Data;
using Apple.Services.ProductAPI.Models;
using Apple.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    //[Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private ResponseDto _response;
        private IMapper _mapper;
        public ProductAPIController(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _response = new ResponseDto();
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objList = _dbContext.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product obj = _dbContext.Products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(obj);
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
        public ResponseDto Post([FromBody] ProductDto ProductDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(ProductDto);
                _dbContext.Products.Add(obj);
                _dbContext.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto ProductDto)
        {
            try
            {
                Product obj = _mapper.Map<Product>(ProductDto);
                _dbContext.Products.Update(obj);
                _dbContext.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(obj);
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
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _dbContext.Products.First(u => u.ProductId == id);
                _dbContext.Products.Remove(obj);
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