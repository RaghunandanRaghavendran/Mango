namespace Mango.Services.OrderAPI.Models.DTOs
{
    public class ShoppingCartDTO
    {
        public CartDTO Cart { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
    }
}
