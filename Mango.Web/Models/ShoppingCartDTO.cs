﻿namespace Mango.Web.Models
{
    public class ShoppingCartDTO
    {
        public CartDTO Cart { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
    }
}
