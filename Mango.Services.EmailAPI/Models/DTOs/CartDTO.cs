﻿
namespace Mango.Services.EmailAPI.Models.DTOs
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

}
