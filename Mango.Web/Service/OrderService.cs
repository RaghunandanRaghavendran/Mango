using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class OrderService : IOrderService
    {
        private readonly IBaseService _baseService;
        public OrderService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseType?> CreateOrderAsync(ShoppingCartDTO cartDto)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartDto,
                Url = StaticDetails.OrderAPIBase+ "/api/order/CreateOrder"
            });
        }

        public async Task<ResponseType?> CreateStripeSessionAsync(StripeRequestDTO stripeRequestDTO)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = stripeRequestDTO,
                Url = StaticDetails.OrderAPIBase + "/api/order/CreateStripeSession"
            });
        }

        public async Task<ResponseType?> ValidateStripeSessionAsync(int orderId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = orderId,
                Url = StaticDetails.OrderAPIBase + "/api/order/ValidateStripeSession"
            });
        }
    }
}
