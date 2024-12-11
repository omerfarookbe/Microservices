using Apple.Web.Models;

namespace Apple.Web.Service.IService
{
    public interface IBaseService
    {
        public Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
