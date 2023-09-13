using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mango.Services.OrderAPI.Models.DTOs
{
    public class OrderDetailsDTO
    {
        public int OrderDetailsId { get; set; }
        public int OrderId { get; set; }
        //public OrderDTO? Order { get; set; }
        public int ProductId { get; set; }
        public ProductDTO? Product { get; set; }
        public int Count { get; set; }
        public string? ProductName { get; set; }
        public double Price { get; set; }
    }
}
