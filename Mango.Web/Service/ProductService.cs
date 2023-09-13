using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
	public class ProductService : IProductService
	{
		private readonly IBaseService _baseService;

        public ProductService(IBaseService baseService)
        {
			_baseService = baseService;
		}
        public async Task<ResponseType?> CreateProductAsync(ProductDTO productDTO)
		{
			return await _baseService.SendAsync(new RequestType()
			{
				ApiType = StaticDetails.ApiType.POST,
				Data = productDTO,
				Url = StaticDetails.ProductAPIBase + "/api/product"
			});
		}

		public async Task<ResponseType?> DeleteProductAsync(int productId)
		{
			return await _baseService.SendAsync(new RequestType()
			{
				ApiType = StaticDetails.ApiType.DELETE,
				Url = StaticDetails.ProductAPIBase + "/api/product/" + productId
			});
		}

		public async Task<ResponseType?> GetProductAsync(string productname)
		{
			return await _baseService.SendAsync(new RequestType()
			{
				ApiType = StaticDetails.ApiType.GET,
				Url = StaticDetails.ProductAPIBase + "/api/product/GetByName/" + productname
			});
		}

		public async Task<ResponseType?> GetProductByIdAsync(int productId)
		{
			return await _baseService.SendAsync(new RequestType()
			{
				ApiType = StaticDetails.ApiType.GET,
				Url = StaticDetails.ProductAPIBase + "/api/product/" + productId
			});
		}

		public async Task<ResponseType?> GetProductsAsync()
		{
			return await _baseService.SendAsync(new RequestType()
			{
				ApiType = StaticDetails.ApiType.GET,
				Url = StaticDetails.ProductAPIBase + "/api/product/"
			},withBearer: true);
		}

		public async Task<ResponseType?> UpdateProduct(ProductDTO productDTO)
		{
			return await _baseService.SendAsync(new RequestType()
			{
				ApiType = StaticDetails.ApiType.PUT,
				Data = productDTO,
				Url = StaticDetails.ProductAPIBase + "/api/product"
			});
		}
	}
}
