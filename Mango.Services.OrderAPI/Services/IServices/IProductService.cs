using Mango.Services.OrderAPI.Models.DTOs;

namespace Mango.ServicesOrderAPI.Services.IServices
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDTO>> GetProducts();
    }
}
