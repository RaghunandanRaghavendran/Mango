using Mango.Services.OrderAPI.Models.DTOs;

namespace Mango.ServicesOrderAPI.Services.IServices
{
    public interface ICartService
    {
        Task<bool> RemoveCartForUser(string userId);
    }
}
