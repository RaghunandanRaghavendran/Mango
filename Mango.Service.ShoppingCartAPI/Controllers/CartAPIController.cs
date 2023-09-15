using AutoMapper;
using Mango.MessageBus;
using Mango.Services.ShoppingCartAPI.Data;
using Mango.Services.ShoppingCartAPI.Models;
using Mango.Services.ShoppingCartAPI.Models.DTOs;
using Mango.Services.ShoppingCartAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ResponseType _responseType;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _config;
        public CartAPIController(IMapper mapper, ApplicationDbContext dbContext,
            ICouponService couponService,
            IProductService productService,
            IMessageBus messageBus,
            IConfiguration config)
        {
            _mapper = mapper;
            _dbcontext = dbContext;

            this._responseType = new();
            _couponService = couponService;
            _productService = productService;
            _messageBus = messageBus;
            _config = config;
        }

        [Authorize]
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseType> GetCart(string userId)
        {
            try
            {
                ShoppingCartDTO cart = new()
                {
                    Cart = _mapper.Map<CartDTO>(_dbcontext.Carts.First(u => u.UserId == userId))
                };

                cart.CartDetails = _mapper.Map<IEnumerable<CartDetailsDTO>>(_dbcontext.CartDetails
                    .Where(u => u.CartId == cart.Cart.CartId));

                IEnumerable<ProductDTO> productDtos = await _productService.GetProducts();

                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cart.Cart.CartTotal += (item.Count * item.Product.Price);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cart.Cart.CouponCode))
                {
                    CouponDTO coupon = await _couponService.GetCoupon(cart.Cart.CouponCode);
                    if (coupon != null && cart.Cart.CartTotal > coupon.MinAmount)
                    {
                        cart.Cart.CartTotal -= coupon.DiscountAmount;
                        cart.Cart.Discount = coupon.DiscountAmount;
                    }
                }

                _responseType.Result = cart;
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.Message;
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("CartUpsert")]
        public async Task<ResponseType> CartUpsert(UpsertShoppingCartDTO cartDTO)
        {
            try
            {
                var cartFromDb = await _dbcontext.Carts.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDTO.Cart.UserId);
                if (cartFromDb == null)
                {
                    // create new cart and cart details
                    Cart cart = _mapper.Map<Cart>(cartDTO.Cart);
                    _dbcontext.Carts.Add(cart);
                    await _dbcontext.SaveChangesAsync();
                    cartDTO.CartDetails.First().CartId = cart.CartId;
                    _dbcontext.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                    await _dbcontext.SaveChangesAsync();
                }
                else
                {
                    // if cart is not null
                    // check if the details has some product
                    var cartDetailsFromDb = await _dbcontext.CartDetails.AsNoTracking().
                        FirstOrDefaultAsync(u => u.ProductId == cartDTO.CartDetails.First().ProductId &&
                        u.CartId == cartFromDb.CartId);
                    if (cartDetailsFromDb == null)
                    {
                        // create cart details
                        cartDTO.CartDetails.First().CartId = cartFromDb.CartId;
                        _dbcontext.CartDetails.Add(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _dbcontext.SaveChangesAsync();
                    }
                    else
                    {
                        //update the count in cart details
                        cartDTO.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDTO.CartDetails.First().CartId = cartDetailsFromDb.CartId;
                        cartDTO.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _dbcontext.CartDetails.Update(_mapper.Map<CartDetails>(cartDTO.CartDetails.First()));
                        await _dbcontext.SaveChangesAsync();
                    }
                }
                _responseType.Result = cartDTO;
            }
            catch (Exception ex)
            {
                _responseType.Message = ex.Message;
                _responseType.IsSuccess = false;
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("RemoveCart")]
        public async Task<ResponseType> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _dbcontext.CartDetails
                   .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _dbcontext.CartDetails.Where(u => u.CartId == cartDetails.CartId).Count();
                _dbcontext.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _dbcontext.Carts
                       .FirstOrDefaultAsync(u => u.CartId == cartDetails.CartId);

                    _dbcontext.Carts.Remove(cartHeaderToRemove);
                }
                await _dbcontext.SaveChangesAsync();

                _responseType.Result = true;
            }
            catch (Exception ex)
            {
                _responseType.Message = ex.Message.ToString();
                _responseType.IsSuccess = false;
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] AddOrRemoveCouponDTO cartDto)
        {
            try
            {
                var cartFromDb = await _dbcontext.Carts.FirstAsync(u => u.UserId == cartDto.UserId);
                cartFromDb.CouponCode = cartDto.CouponCode;
                _dbcontext.Carts.Update(cartFromDb);
                await _dbcontext.SaveChangesAsync();
                _responseType.Result = true;
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.ToString();
            }
            return _responseType;
        }
        [Authorize]
        [HttpPost("RemoveCoupon")]
        public async Task<object> RemoveCoupon([FromBody] AddOrRemoveCouponDTO cartDto)
        {
            try
            {
                var cartFromDb = await _dbcontext.Carts.FirstAsync(u => u.UserId == cartDto.UserId);
                cartFromDb.CouponCode = "";
                _dbcontext.Carts.Update(cartFromDb);
                await _dbcontext.SaveChangesAsync();
                _responseType.Result = true;
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.ToString();
            }
            return _responseType;
        }

        [Authorize]
        [Route("RemoveCartByUser/{userId}")]
        [HttpDelete]
        public async Task<ResponseType> RemoveCartByUserID(string userId)
        {
            try
            {
                Cart cart = _dbcontext.Carts.SingleOrDefault(c => c.UserId == userId);
                if (cart != null)
                {
                    IEnumerable<CartDetails> cartDetails = _dbcontext.CartDetails.Where(u => u.CartId == cart.CartId);
                    foreach (var cartDetail in cartDetails)
                    {
                        _dbcontext.Remove(cartDetail);
                    }
                    int removeCartDetailsCount = await _dbcontext.SaveChangesAsync();

                    if (removeCartDetailsCount > 0)
                    {
                        _dbcontext.Remove(cart);
                        await _dbcontext.SaveChangesAsync();
                    }
                }
                _responseType.Result = true;
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.ToString();
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("EmailCartRequest")]
        public async Task<object> EmailCartRequest([FromBody] ShoppingCartDTO cartDto)
        {
            try
            {
                try
                {
                    await _messageBus.PublishMessage(cartDto, _config.GetValue<string>("TopicAndQueueNames:EmailShoppingCartQueue"));
                    _responseType.Result = true;
                }
                catch (Exception ex)
                {
                    _responseType.IsSuccess = false;
                    _responseType.Message = ex.ToString();
                }

            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.ToString();
            }
            return _responseType;
        }
    }
}
