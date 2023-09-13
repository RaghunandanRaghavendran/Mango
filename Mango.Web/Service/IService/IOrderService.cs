using Mango.Web.Models;
using System.Threading.Tasks;

namespace Mango.Web.Service.IService
{
    public interface IOrderService
    {
        Task<ResponseType?> CreateOrderAsync(ShoppingCartDTO shoppingCartDTO);
        Task<ResponseType?> CreateStripeSessionAsync(StripeRequestDTO stripeRequestDTO);
        Task<ResponseType?> ValidateStripeSessionAsync(int orderId);
        Task<ResponseType?> GetAllOrder(string? userId);
        Task<ResponseType?> GetOrder(int orderId);
        Task<ResponseType?> UpdateOrderStatus(int orderId, string newStatus);

    }
}
