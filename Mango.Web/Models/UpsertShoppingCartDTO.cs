namespace Mango.Web.Models
{
    public class UpsertShoppingCartDTO
    {
        public UpsertCartDTO Cart { get; set; }
        public IEnumerable<UpsertCartDetailsDTO>? CartDetails { get; set; }
    }
}
