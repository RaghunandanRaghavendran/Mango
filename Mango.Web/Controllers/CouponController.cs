using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;
        public CouponController(ICouponService couponService)
        {
                _couponService = couponService;
        }
        #region Fetch Coupon
        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO>? couponDTOs = new();
            ResponseType? response = await _couponService.GetCouponsAsync();
            if (response != null && response.IsSuccess) 
            {
                couponDTOs = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(couponDTOs);
        }
        #endregion

        #region Create Coupon section
        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDTO couponDTO)
        {
            if(ModelState.IsValid)
            {
                ResponseType? response = await _couponService.CreateCouponAsync(couponDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "New Coupon " + couponDTO.CouponCode + " created";
                    return RedirectToAction(nameof(CouponIndex));                 
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(couponDTO);
        }
        #endregion

        #region Delete Coupon section
        public async Task<IActionResult> CouponDelete(int couponId)
        {
            ResponseType? response = await _couponService.GetCouponByIdAsync(couponId);
            if (response != null && response.IsSuccess)
            {
                CouponDTO couponDTO = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(couponDTO);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }       

        [HttpPost]
        public async Task<IActionResult> CouponDelete(CouponDTO couponDTO)
        {
            ResponseType? response = await _couponService.DeleteCouponAsync(couponDTO.CouponId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon deleted successfully";
                return RedirectToAction(nameof(CouponIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }
        #endregion
    }
}
