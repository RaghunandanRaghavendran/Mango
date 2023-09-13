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
        [Authorize]
        public ActionResult<ResponseType> Post([FromBody] ProductDTO productDTO)
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

				_response.Result = _mapper.Map<ProductDTO>(product);

			}
			catch (Exception ex)
			{
				_response.Message = ex.Message;
				_response.IsSuccess = false;
			}
			return CreatedAtRoute("GetProductById", new { id = product.ProductId }, _response); ;
		}

		[HttpPut]
        [Authorize]
        public ActionResult<ResponseType> Put([FromBody] ProductDTO productDTO)
		{
			try
			{
				if (productDTO == null)
				{
					return BadRequest();
				}

				Product product = _mapper.Map<Product>(productDTO);

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
        [Authorize]
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
