namespace Mango.Services.ShoppingCartAPI.Models.DTOs
{
    public class UpsertCartDTO
    {
        public string? UserId { get; set; }
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
    }
}
