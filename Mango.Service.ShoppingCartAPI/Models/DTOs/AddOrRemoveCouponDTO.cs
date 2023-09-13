namespace Mango.Services.ShoppingCartAPI.Models.DTOs
{
    public class AddOrRemoveCouponDTO
    {
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
    }
}
