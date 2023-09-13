using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
            _baseService= baseService;
        }
        public async Task<ResponseType?> CreateCouponAsync(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.POST,
                Data = couponDTO,
                Url = StaticDetails.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseType?> DeleteCouponAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.DELETE,
                Url = StaticDetails.CouponAPIBase + "/api/coupon/" + couponId
            });
        }

        public async Task<ResponseType?> GetCouponAsync(string couponcode)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponAPIBase + "/api/coupon/GetByCode/"+couponcode
            });
        }

        public async Task<ResponseType?> GetCouponByIdAsync(int couponId)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponAPIBase + "/api/coupon/" +couponId
            }) ;
        }

        public async Task<ResponseType?> GetCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.GET,
                Url = StaticDetails.CouponAPIBase + "/api/coupon"
            });
        }

        public async Task<ResponseType?> UpdateCoupon(CouponDTO couponDTO)
        {
            return await _baseService.SendAsync(new RequestType()
            {
                ApiType = StaticDetails.ApiType.PUT,
                Data = couponDTO,
                Url = StaticDetails.CouponAPIBase + "/api/coupon"
            });
        }
    }
}
