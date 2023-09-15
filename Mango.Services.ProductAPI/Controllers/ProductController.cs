using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Controllers
{
	[Route("api/product")]
	[ApiController]
	public class ProductController: ControllerBase
	{
		private readonly ApplicationDbContext _dbContext;
		private ResponseType _response;
		private IMapper _mapper;
		public ProductController(ApplicationDbContext dbContext, IMapper mapper)
        {
			_dbContext = dbContext;
			_mapper = mapper;
			_response = new ResponseType();
		}

		[HttpGet]
		public ActionResult<ResponseType> Get()
		{
			try
			{
				IEnumerable<Product> products = _dbContext.Products.ToList();
				_response.Result = _mapper.Map<IEnumerable<ProductDTO>>(products);
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
			return Ok(_response);
		}

		[HttpGet]
        [Authorize]
        [Route("{id:int}", Name = "GetProductById")]
		public ActionResult<ResponseType> Get(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}

				Product product = _dbContext.Products.FirstOrDefault(x => x.ProductId == id);
				if (product == null)
				{
					return NotFound();
				}
				_response.Result = _mapper.Map<ProductDTO>(product);
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
			return Ok(_response);
		}

		[HttpGet]
        [Authorize]
        [Route("GetByName/{productname}:string")]
		public ResponseType GetByCode(string productname)
		{
			try
			{
				Product product = _dbContext.Products.FirstOrDefault(x => x.Name.ToLower() == productname.ToLower());
				_response.Result = _mapper.Map<ProductDTO>(product);
			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
			return _response;
		}

		[HttpPost]
        [Authorize(Roles ="ADMIN")]
        public ActionResult<ResponseType> Post([FromForm]ProductDTO productDTO)
		{
			Product product = new Product();

            try
			{
                if (productDTO == null)
                {
                    return BadRequest();
                }

                product = _mapper.Map<Product>(productDTO);
				_dbContext.Products.Add(product);
				_dbContext.SaveChanges();

				if(productDTO.Image != null)
				{
					string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
					string filePath = @"wwwroot\ProductImages\" + fileName;
					var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);

					using(var fileStream = new FileStream(filePathDirectory, FileMode.Create))
					{
						productDTO.Image.CopyTo(fileStream);
					}

                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
				else
				{
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _dbContext.Products.Update(product);
                _dbContext.SaveChanges();
                _response.Result = _mapper.Map<ProductDTO>(product);

			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
            return CreatedAtRoute("GetProductById", new { id = product.ProductId }, _response);
        }

		[HttpPut]
        [Authorize(Roles ="ADMIN")]
        public ActionResult<ResponseType> Put([FromForm] ProductDTO productDTO)
		{
			try
			{
				if (productDTO == null)
				{
					return BadRequest();
				}

				Product product = _mapper.Map<Product>(productDTO);

                if (productDTO.Image != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageLocalPath))
                    {
                        var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                        FileInfo file = new FileInfo(oldFilePathDirectory);
                        if (file.Exists)
                        {
                            file.Delete();
                        }
                    }

                    string fileName = product.ProductId + Path.GetExtension(productDTO.Image.FileName);
                    string filePath = @"wwwroot\ProductImages\" + fileName;
                    var filePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                    using (var fileStream = new FileStream(filePathDirectory, FileMode.Create))
                    {
                        productDTO.Image.CopyTo(fileStream);
                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }


                _dbContext.Products.Update(product);
				_dbContext.SaveChanges();


				_response.Result = _mapper.Map<ProductDTO>(product);

			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
			return Ok(_response);
		}

		[HttpDelete]
        [Authorize(Roles ="ADMIN")]
        [Route("{id:int}")]
		public ActionResult<ResponseType> Delete(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest();
				}

				Product product = _dbContext.Products.FirstOrDefault(x => x.ProductId == id);

				if (product == null)
				{
					return NotFound();
				}

                if (!string.IsNullOrEmpty(product.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), product.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }

                _dbContext.Products.Remove(product);
				_dbContext.SaveChanges();

			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
			return _response;
		}
	}
}
