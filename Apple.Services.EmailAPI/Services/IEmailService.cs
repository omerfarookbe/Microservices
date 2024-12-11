using Apple.Services.EmailAPI.Models.Dtos;

namespace Apple.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        public Task EmailCartAndLog(CartDto cartDto);
    }
}
