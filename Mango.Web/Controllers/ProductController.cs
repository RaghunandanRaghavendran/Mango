using Mango.Web.Models;
using Mango.Web.Service;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductService _productService;
		public ProductController(IProductService productService)
		{
			_productService = productService;
		}
		public async Task<IActionResult> ProductIndex()
		{
			List<ProductDTO>? productDTOs = new();
			ResponseType? response = await _productService.GetProductsAsync();
			if (response != null && response.IsSuccess)
			{
				productDTOs = JsonConvert.DeserializeObject<List<ProductDTO>>(Convert.ToString(response.Result));
			}
			else
			{
				TempData["error"] = response?.Message;
			}
			return View(productDTOs);
		}

        #region Create Product
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseType? response = await _productService.CreateProductAsync(productDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "New Product " + productDTO.Name + " created";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDTO);
        }
        #endregion

        #region Edit Product
        public async Task<IActionResult> ProductEdit(int productId)
        {
			ResponseType? response = await _productService.GetProductByIdAsync(productId);
			if (response != null && response.IsSuccess)
			{
				ProductDTO productDTO = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
				return View(productDTO);
			}
			else
			{
				TempData["error"] = response?.Message;
			}

			return NotFound();
		}
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDTO productDTO)
        {
            if (ModelState.IsValid)
            {
                ResponseType? response = await _productService.UpdateProduct(productDTO);
                if (response != null && response.IsSuccess)
                {
                    TempData["updated"] = "Product " + productDTO.Name + " edited";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDTO);
        }
        #endregion

        #region Delete Product
        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseType? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                ProductDTO productDTO = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
                return View(productDTO);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDTO productDTO)
        {
            ResponseType? response = await _productService.DeleteProductAsync(productDTO.ProductId);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
        #endregion
    }
}
