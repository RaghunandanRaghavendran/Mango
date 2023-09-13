using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController: ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private ResponseType _response;
        private IMapper _mapper;
        public CouponAPIController(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _response = new ResponseType();
        }
        [HttpGet]
        [Authorize]
        public ActionResult<ResponseType> Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = _dbContext.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDTO>>(coupons);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return Ok(_response);
        }

        [HttpGet]
        [Authorize]
        [Route("{id:int}", Name ="GetCouponById")]
        public ActionResult<ResponseType> Get(int id)
        {
            try
            {
                if(id == 0)
                {
                    return BadRequest();
                }

                Coupon coupon = _dbContext.Coupons.FirstOrDefault(x=>x.CouponId == id);
                if(coupon == null)
                {
                    return NotFound();
                }
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return Ok(_response);
        }

        [HttpGet]
        [Authorize]
        [Route("GetByCode/{code}")]
        public ResponseType GetByCode(string code)
        {
            try
            {
                Coupon coupon = _dbContext.Coupons.FirstOrDefault(x => x.CouponCode .ToLower() == code.ToLower());
                _response.Result = _mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult<ResponseType> Post([FromBody] CouponDTO couponDto)
        {
            Coupon coupon = new Coupon();
            try
            {              
                if(couponDto == null)
                {
                    return BadRequest();
                }

                 coupon = _mapper.Map<Coupon>(couponDto);
                _dbContext.Coupons.Add(coupon);
                _dbContext.SaveChanges();

                // Stripe changes - Updating the Coupon in the stripe portal
                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)couponDto.DiscountAmount * 100,
                    Name = couponDto.CouponCode,
                    Currency = "inr",
                    Id = couponDto.CouponCode,
                };
                var service = new Stripe.CouponService();
                service.Create(options);

                _response.Result = _mapper.Map<CouponDTO>(coupon);

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return CreatedAtRoute("GetCouponById", new { id = coupon.CouponId }, _response); ;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult<ResponseType> Put([FromBody] CouponDTO couponDto)
        {
            try
            {
                if(couponDto == null)
                {
                    return BadRequest();
                }
                
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _dbContext.Coupons.Update(coupon);
                _dbContext.SaveChanges();

                _response.Result = _mapper.Map<CouponDTO>(coupon);

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return Ok(_response);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<ResponseType> Delete(int id)
        {
            try
            {
                if(id == 0)
                {
                    return BadRequest();
                }
                
                Coupon coupon = _dbContext.Coupons.FirstOrDefault(x=>x.CouponId == id);

                if(coupon == null)
                {
                    return NotFound();
                }
                _dbContext.Coupons.Remove(coupon);
                _dbContext.SaveChanges();

                //stripe - Delete from Stripe
                var service = new Stripe.CouponService();
                service.Delete(coupon.CouponCode);

            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSuccess = false;
            }
            return _response;
        }

    }
}
