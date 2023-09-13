namespace Mango.Services.EmailAPI.Models.DTOs
{
    public class ShoppingCartDTO
    {
        public CartDTO Cart { get; set; }
        public IEnumerable<CartDetailsDTO>? CartDetails { get; set; }
    }
}
