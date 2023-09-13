using IdentityModel;
using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, IProductService productService, ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
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

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDTO? productDTO = new();
            ResponseType? response = await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSuccess)
            {
                productDTO = JsonConvert.DeserializeObject<ProductDTO>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDTO);

        }

        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDTO productDTO)
        {
            UpsertShoppingCartDTO cartDTO = new UpsertShoppingCartDTO()
            {
                Cart = new UpsertCartDTO
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };
            UpsertCartDetailsDTO cartDetailsDTO = new UpsertCartDetailsDTO()
            {
                Count = productDTO.Count,
                ProductId = productDTO.ProductId,

            };
            List<UpsertCartDetailsDTO> cartDetails = new() { cartDetailsDTO };
            cartDTO.CartDetails = cartDetails;


            ResponseType? response = await _cartService.UpsertCartAsync(cartDTO);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to the Shopping cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDTO);

        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}