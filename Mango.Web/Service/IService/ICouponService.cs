using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        Task <ResponseType?> GetCouponAsync(string couponcode);
        Task<ResponseType?> GetCouponsAsync();
        Task<ResponseType?> GetCouponByIdAsync(int couponId);
        Task<ResponseType?> CreateCouponAsync(CouponDTO couponDTO);
        Task<ResponseType?> UpdateCoupon(CouponDTO couponDTO);
        Task<ResponseType?> DeleteCouponAsync(int couponId);

    }
}
