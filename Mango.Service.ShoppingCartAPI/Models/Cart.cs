﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.ShoppingCartAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }
        public string? UserId { get; set; }

        public string? CouponCode { get; set; }
        [NotMapped]
        public double Discount { get; set; }
        [NotMapped]
        public double CartTotal { get; set; }
    }
}
