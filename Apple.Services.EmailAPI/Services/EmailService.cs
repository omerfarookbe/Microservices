using Apple.Services.EmailAPI.Data;
using Apple.Services.EmailAPI.Models;
using Apple.Services.EmailAPI.Models.Dtos;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Apple.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbOptions;

        public EmailService(DbContextOptions<AppDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        public async Task EmailCartAndLog(CartDto cartDto)
        {
            StringBuilder message = new StringBuilder();
            message.Append("<br/>Email cart requested");
            message.Append("<br/>Total" + cartDto.CartHeader.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");
            foreach (var item in cartDto.CartDetails)
            {
                message.Append("<li>");
                message.Append(item.Product.Name + "x" + item.Count);
                message.Append("</li>");
            }
            message.Append("</ul>");
            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email);
        }

        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    Message = message,
                    EmailSent = DateTime.Now
                };
                await using var _db = new AppDbContext(_dbOptions);
                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

    }
}
