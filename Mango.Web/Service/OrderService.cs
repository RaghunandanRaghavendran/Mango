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

        public async Task<ResponseType?> GetAllOrder(string? userId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.OrderAPIBase + "/api/order/GetOrders?userId=" +userId
			});
        }

		public async Task<ResponseType?> GetOrder(int orderId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.OrderAPIBase + "/api/order/GetOrder/" + orderId
            });
        }

        public async Task<ResponseType?> UpdateOrderStatus(int orderId, string newStatus)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = newStatus,
                Url = StaticDetails.OrderAPIBase + "/api/order/UpdateOrderStatus/" +orderId
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
