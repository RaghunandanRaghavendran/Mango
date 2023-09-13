using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace Mango.Web.Controllers
{
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public CartController(ICartService cartService, IOrderService orderService)
        {
            _cartService = cartService;   
            _orderService = orderService;
        }
        [Authorize]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartForLoggedInUser());
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            ResponseType? responseType = await _cartService.RemoveFromCartAsync(cartDetailsId);
            if (responseType?.Result != null && responseType.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(ShoppingCartDTO cartDTO)
        {
            AddOrRemoveCouponDTO couponDTO = new AddOrRemoveCouponDTO()
            {
                UserId = cartDTO.Cart.UserId,
                CouponCode = cartDTO.Cart.CouponCode
            };
            ResponseType? responseType = await _cartService.ApplyCouponAsync(couponDTO);
            if (responseType?.Result != null && responseType.IsSuccess)
            {
                TempData["success"] = "Coupon Added successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(ShoppingCartDTO cartDTO)
        {
            AddOrRemoveCouponDTO couponDTO = new AddOrRemoveCouponDTO()
            {
                UserId = cartDTO.Cart.UserId,
                CouponCode = ""
            };
            ResponseType? responseType = await _cartService.ApplyCouponAsync(couponDTO);
            if (responseType?.Result != null && responseType.IsSuccess)
            {
                TempData["success"] = "Coupon Added successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailCart(ShoppingCartDTO cartDTO)
        {
            cartDTO = await LoadCartForLoggedInUser();
            cartDTO.Cart.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
            cartDTO.Cart.Name = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Name)?.FirstOrDefault()?.Value;
            ResponseType? responseType = await _cartService.EmailCartAsync(cartDTO);
            if (responseType?.Result != null && responseType.IsSuccess)
            {
                TempData["success"] = "Email with be processed and sent shortly.";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<ShoppingCartDTO> LoadCartForLoggedInUser()
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseType? responseType = await _cartService.GetCartByUserIdAsnyc(userId);
            if (responseType?.Result != null && responseType.IsSuccess)
            {
                ShoppingCartDTO? cartDto = JsonConvert.DeserializeObject<ShoppingCartDTO>(responseType.Result.ToString());
                cartDto.Cart.Name = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Name)?.FirstOrDefault()?.Value;
                cartDto.Cart.Email = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Email)?.FirstOrDefault()?.Value;
                return cartDto;
            }
            return new ShoppingCartDTO();                     
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            return View(await LoadCartForLoggedInUser());
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseType? response = await _orderService.ValidateStripeSessionAsync(orderId);
            if (response != null & response.IsSuccess)
            {

                OrderDTO orderHeader = JsonConvert.DeserializeObject<OrderDTO>(Convert.ToString(response.Result));
                if (orderHeader.Status == StaticDetails.STATUS_APPROVED)
                {
                    return View(orderId);
                }
            }
            //redirect to some error page based on status
            return View(orderId);
        }

        [HttpPost]
        [ActionName("Checkout")]
        public async Task<IActionResult> Checkout(ShoppingCartDTO cartDTO)
        {
            ShoppingCartDTO cart = await LoadCartForLoggedInUser();
            cart.Cart.Phone = cartDTO.Cart.Phone;

            var response = await _orderService.CreateOrderAsync(cart);
            OrderDTO orderDTO = JsonConvert.DeserializeObject<OrderDTO>(Convert.ToString(response.Result));
            if(response != null && response.IsSuccess)
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                StripeRequestDTO stripeRequestDTO = new()
                {
                    ApprovedURL = domain + "cart/Confirmation?orderId=" + orderDTO.OrderId,
                    CancelURL = domain + "cart/checkout",
                    Order = orderDTO
                };
                var stripeSession= await _orderService.CreateStripeSessionAsync(stripeRequestDTO);
                StripeRequestDTO stripeResponse = JsonConvert.DeserializeObject<StripeRequestDTO>(Convert.ToString(stripeSession.Result));
                Response.Headers.Add("Location", stripeResponse.StripeSessionURL);
                return new StatusCodeResult(303);

            }
            return View(cart);
        }
    }
}
