using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
	public interface IProductService
	{
		Task<ResponseType?> GetProductAsync(string productname);
		Task<ResponseType?> GetProductsAsync();
		Task<ResponseType?> GetProductByIdAsync(int productId);
		Task<ResponseType?> CreateProductAsync(ProductDTO productDTO);
		Task<ResponseType?> UpdateProduct(ProductDTO productDTO);
		Task<ResponseType?> DeleteProductAsync(int productId);
	}
}
