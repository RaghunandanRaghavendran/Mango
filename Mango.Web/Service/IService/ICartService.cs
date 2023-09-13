using Mango.Web.Models;
using System.Threading.Tasks;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        Task<ResponseType?> GetCartByUserIdAsnyc(string userId);
        Task<ResponseType?> UpsertCartAsync(UpsertShoppingCartDTO cartDto);
        Task<ResponseType?> RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseType?> ApplyCouponAsync(AddOrRemoveCouponDTO cartDto);
        Task<ResponseType?> EmailCartAsync(ShoppingCartDTO cartDto);
    }
}
