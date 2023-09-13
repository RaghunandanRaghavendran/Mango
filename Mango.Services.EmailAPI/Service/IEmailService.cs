using Mango.Services.EmailAPI.Models.DTOs;

namespace Mango.Services.EmailAPI.Service
{
    public interface IEmailService
    {
        Task EmailAndLog(ShoppingCartDTO cartDTO);
        Task LogOrderPlaced(OrderPlacedEmailNotificationDTO orderDTO);
    }
}
