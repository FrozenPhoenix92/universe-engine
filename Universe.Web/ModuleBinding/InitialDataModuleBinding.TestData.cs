using Bogus;

using Microsoft.EntityFrameworkCore;

using Universe.Core.Data;
using Universe.Web.Model;

namespace Universe.Web.ModuleBinding;

public partial class InitialDataModuleBinding
{
	private static async Task SeedTestData(IDbContext context)
	{
		var customers = await context.Set<Customer>().ToListAsync();
		if (!customers.Any())
		{
			customers = GenerateCustomers();
			await context.Set<Customer>().AddRangeAsync(customers);
		}

		var products = await context.Set<Product>().ToListAsync();
		if (!products.Any())
		{
			products = GenerateProducts();
			await context.Set<Product>().AddRangeAsync(products);
		}

		await context.SaveChangesAsync();

		var orders = await context.Set<Order>().ToListAsync();
		if (!orders.Any())
		{
			orders = GenerateOrders();
			await context.Set<Order>().AddRangeAsync(orders);
		}

		await context.SaveChangesAsync();


		List<Order> GenerateOrders()
		{
			var generator = new Faker<Order>()
				.RuleFor(p => p.Created, s => s.Date.Past(1, DateTime.UtcNow))
				.RuleFor(p => p.LastModified, (f, current) => current.Created.AddMonths(f.Random.Number(0, 3)))
				.RuleFor(p => p.Customer, s => s.PickRandom(customers))
				.RuleFor(p => p.Status, s => s.PickRandom<OrderStatus>())
				.RuleFor(p => p.OrderLines, s => GenerateOrderLines());

			return generator.Generate(1000).ToList();


			List<OrderLine> GenerateOrderLines()
			{
				var productsListCopy = new List<Product>(products);
				var linesGenerator = new Faker<OrderLine>()
					.RuleFor(p => p.Count, s => s.Random.Number(1, 5))
					.RuleFor(p => p.Product, s => s.PickRandom(productsListCopy))
					.RuleFor(p => p.Price, (f, current) => current.Count * (current.Product?.Price ?? 0))
					.FinishWith((f, current) => productsListCopy.Remove(current.Product!));

				return linesGenerator.Generate(new Random().Next(1, Math.Min(productsListCopy.Count, 20))).ToList();
			}
		}

		List<Customer> GenerateCustomers() 
		{
			var generator = new Faker<Customer>()
				.RuleFor(p => p.Address, s => s.Address.FullAddress())
				.RuleFor(p => p.Name, s => s.Name.FirstName())
				.RuleFor(p => p.LastName, s => s.Name.LastName())
				.RuleFor(p => p.Photo, s => s.Image.PicsumUrl());

			return generator.Generate(10).ToList();
		}

		List<Product> GenerateProducts()
		{
			var generator = new Faker<Product>()
				.RuleFor(p => p.Name, s => s.Commerce.Product())
				.RuleFor(p => p.Price, s => (double) s.Random.Number(1, 100000) / 100);

			return generator.Generate(100).DistinctBy(x => x.Name).ToList();
		}
	}
}
