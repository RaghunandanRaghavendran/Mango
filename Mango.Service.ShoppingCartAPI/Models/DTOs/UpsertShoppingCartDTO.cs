namespace Mango.Services.ShoppingCartAPI.Models.DTOs
{
    public class UpsertShoppingCartDTO
    {
        public UpsertCartDTO Cart { get; set; }
        public IEnumerable<UpsertCartDetailsDTO>? CartDetails { get; set; }
    }
}
