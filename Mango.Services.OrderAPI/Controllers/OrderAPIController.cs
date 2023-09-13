using AutoMapper;
using Mango.ServicesOrderAPI.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Mango.Services.OrderAPI.Utility;
using Mango.Services.OrderAPI.Models;
using Azure;
using Stripe.Checkout;
using Mango.MessageBus;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace Mango.Services.OrderAPI.Controllers
{
    [ApiController]
    [Route("api/order")]
    public class OrderAPIController: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ResponseType _responseType;
        private readonly ApplicationDbContext _dbcontext;
        private readonly IProductService _productService;
        private readonly IConfiguration _config;
        private readonly IMessageBus _messageBus;
        public OrderAPIController(IMapper mapper, ApplicationDbContext dbContext,
           IProductService productService,
           IConfiguration config,
           IMessageBus messageBus)
        {
            _mapper = mapper;
            _dbcontext = dbContext;
            this._responseType = new();
            _productService = productService;
            _config = config;
            _messageBus = messageBus;
        }

        [Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseType> CreateOrder([FromBody] ShoppingCartDTO cartDTO)
        {
            try
            {
                OrderDTO orderDTO = _mapper.Map<OrderDTO>(cartDTO.Cart);
                orderDTO.OrderTime = DateTime.Now;
                orderDTO.Status = StaticDetails.STATUS_PENDING;
                orderDTO.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDTO>>(cartDTO.CartDetails);

                Order order = _dbcontext.Orders.Add(_mapper.Map<Order>(orderDTO)).Entity;
                var isOrderCreated = await _dbcontext.SaveChangesAsync();

                if(isOrderCreated > 0)
                {
                    orderDTO.OrderId = order.OrderId;
                    _responseType.Result = orderDTO;
                }
                else
                {
                    _responseType.IsSuccess = false;
                    _responseType.Message = "Order creation failed to save";
                }
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.Message;
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseType> CreateStripeSession([FromBody] StripeRequestDTO stripeRequestDto)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedURL,
                    CancelUrl = stripeRequestDto.CancelURL,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon=stripeRequestDto.Order.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.Order.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "inr",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product?.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.Order.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }
                var service = new SessionService();
                Session session = service.Create(options); // This is a stripe session and not the .net session
                stripeRequestDto.StripeSessionURL = session.Url;
                Order orderHeader = _dbcontext.Orders.First(u => u.OrderId == stripeRequestDto.Order.OrderId);
                orderHeader.StripeSessionId = session.Id;
                await _dbcontext.SaveChangesAsync();
                _responseType.Result = stripeRequestDto;
            }
            catch (Exception ex)
            {
                _responseType.Message = ex.Message;
                _responseType.IsSuccess = false;
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseType> ValidateStripeSession([FromBody] int orderId)
        {
            try
            {
                Order order = _dbcontext.Orders.First(u => u.OrderId == orderId);

                var service = new SessionService();
                Session session = service.Get(order.StripeSessionId);

                var paymentIntentService = new Stripe.PaymentIntentService();
                Stripe.PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //then payment was successful
                    order.PaymentIntentId = paymentIntent.Id;
                    order.Status = StaticDetails.STATUS_APPROVED;
                    _dbcontext.SaveChanges();
                    RewardsDTO rewardsDTO = new RewardsDTO()
                    {
                        OrderId = order.OrderId,
                        RewardPoints = Convert.ToInt32(Math.Round(order.OrderTotal / 10)),
                        RewardsDate = DateTime.Now,
                        UserId= order.UserId,
                    };
                    // publish message for rewards and Email
                    string topicName = _config.GetValue<string>("TopicAndQueueNames:OrderCreatedTopic");
                    await _messageBus.PublishMessage(rewardsDTO, topicName);
                    _responseType.Result = _mapper.Map<OrderDTO>(order);
                }
            }
            catch (Exception ex)
            {
                _responseType.Message = ex.Message;
                _responseType.IsSuccess = false;
            }
            return _responseType;
        }
        
        [Authorize]
        [HttpGet("GetOrders")]
        public async Task<ResponseType?> Get(string? userId = "")
        {
            try
            {
                IEnumerable<Order> orders;
                if (User.IsInRole(StaticDetails.ROLE_ADMIN))
                {
                     orders = await _dbcontext.Orders.Include(u => u.OrderDetails).OrderByDescending(u => u.OrderId).ToListAsync();
                }
                else
                {
                    orders = await _dbcontext.Orders.Include(u => u.OrderDetails).Where(u => u.UserId == userId).OrderByDescending(u => u.OrderId).ToListAsync();
                }
                _responseType.Result = _mapper.Map<IEnumerable<OrderDTO>>(orders);
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.Message;
            }
            return _responseType;
        }

        [Authorize]
        [HttpGet("GetOrder/{id:int}")]
        public async Task<ResponseType?> Get(int id)
        {
            try
            {
                Order orderHeader = await _dbcontext.Orders.Include(u => u.OrderDetails).FirstAsync(u => u.OrderId == id);
                _responseType.Result = _mapper.Map<OrderDTO>(orderHeader);
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
                _responseType.Message = ex.Message;
            }
            return _responseType;
        }

        [Authorize]
        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        public async Task<ResponseType> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                Order order = _dbcontext.Orders.First(u => u.OrderId == orderId);
                if (order != null)
                {
                    if (newStatus == StaticDetails.STATUS_CANCELLED)
                    {
                        //we will give refund
                        var options = new RefundCreateOptions
                        {
                            Reason = RefundReasons.RequestedByCustomer,
                            PaymentIntent = order.PaymentIntentId
                        };

                        var service = new RefundService();
                        Refund refund = service.Create(options);
                    }
                    order.Status = newStatus;
                    _dbcontext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _responseType.IsSuccess = false;
            }
            return _responseType;
        }
    }
}
