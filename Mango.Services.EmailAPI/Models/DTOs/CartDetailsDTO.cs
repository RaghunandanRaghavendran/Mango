using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Mango.Services.EmailAPI.Models.DTOs
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
