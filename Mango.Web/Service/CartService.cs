using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
            _baseService = baseService;
        }
        public async Task<ResponseType?> ApplyCouponAsync(AddOrRemoveCouponDTO cartDto)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartDto,
                Url = StaticDetails.CartAPIBase+ "/api/cart/ApplyCoupon"
            });
        }

        public async Task<ResponseType?> EmailCartAsync(ShoppingCartDTO cartDto)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartDto,
                Url = StaticDetails.CartAPIBase + "/api/cart/EmailCartRequest"
            });
        }

        public async Task<ResponseType?> GetCartByUserIdAsnyc(string userId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CartAPIBase + "/api/cart/GetCart/" + userId
            });
        }

        public async Task<ResponseType?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartDetailsId,
                Url = StaticDetails.CartAPIBase + "/api/cart/RemoveCart"
            });
        }

        public async Task<ResponseType?> UpsertCartAsync(UpsertShoppingCartDTO cartDto)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = cartDto,
                Url = StaticDetails.CartAPIBase + "/api/cart/CartUpsert"
            });
        }
    }
}
