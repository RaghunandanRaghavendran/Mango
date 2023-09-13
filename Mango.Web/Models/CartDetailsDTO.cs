
namespace Mango.Web.Models
{
    public class CartDetailsDTO
    {
        public int CartDetailsId { get; set; }
        public int CartId { get; set; }
        public CartDTO? Cart { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }

    }
}
