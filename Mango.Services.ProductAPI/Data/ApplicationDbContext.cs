using Mango.Services.ProductAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.ProductAPI.Data
{
	public class ApplicationDbContext: DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}
		public DbSet<Product> Products { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Product>().HasData(new Product
			{
				ProductId = 1,
				Name = "Samosa",
				Price = 15,
				Description = " A samosa is a fried South Asian pastry with a savoury filling, including ingredients such as spiced potatoes, onions, peas, meat or fish. It may take different forms, including triangular, cone, or half-moon shapes, depending on the region. Samosas are often accompanied by chutney, and have origins in medieval times or earlier. Sweet versions are also made.",
				ImageUrl = "https://placehold.co/603x403",
				CategoryName = "Appetizer"
			});
			modelBuilder.Entity<Product>().HasData(new Product
			{
				ProductId = 2,
				Name = "Paneer Tikka",
				Price = 13.99,
				Description = " Paneer Tikka is a popular and delicious tandoori snack where Paneer (Indian cottage cheese cubes) are marinated in a spiced yogurt-based marinade, arranged on skewers and grilled in the oven. Worry not if you don’t have an oven – instead of grilling in oven, you can make Paneer Tikka Recipe on stovetop on a tawa/skillet. In this post I am sharing both the oven and stovetop methods.",
				ImageUrl = "https://placehold.co/602x402",
				CategoryName = "Appetizer"
			});
			modelBuilder.Entity<Product>().HasData(new Product
			{
				ProductId = 3,
				Name = "Sweet Pie",
				Price = 10.99,
				Description = " Fruity, nutty or chocolatey, decked out with pastry, meringue or crumble, these sumptuous desserts are sure to satisfy your sweet tooth.",
				ImageUrl = "https://placehold.co/601x401",
				CategoryName = "Dessert"
			});
			modelBuilder.Entity<Product>().HasData(new Product
			{
				ProductId = 4,
				Name = "Pav Bhaji",
				Price = 15,
				Description = " Pav bhaji is a street food dish from India consisting of a thick vegetable curry (bhaji) served with a soft bread roll (pav). It originated in the city of Mumbai.",
				ImageUrl = "https://placehold.co/600x400",
				CategoryName = "Entree"
			});

		}
	}
}
