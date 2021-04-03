using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Export;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
	public class Serializer
	{
		public static string ExportOrdersByEmployee(FastFoodDbContext context, string employeeName, string orderType)
		{
			//The given method in the project skeleton receives an employee name and an order type as strings
			//	.Export all orders that were processed by the employee with that name,
			//	which have that order type
			//	.For each order, get the customer’s name and the order’s items with their name,
			//	price and quantity
			//	.Apart from that, for every order, also list the total price of the order
			//	.Sort the orders by their total price(descending),
			//	then by the number of items in the order(descending)
			//	.Finally, also export the total money made from all the orders.

			var orders = context.Employees
				.ToArray()
				.Where(x => x.Name == employeeName)
				.Select(c => new
				{
					Name = c.Name,
					Orders = c.Orders.Where(x => x.Type.ToString() == orderType)
					.Select(x => new
					{
						Name = x.Customer,
						Items = x.OrderItems.
						Select(y => new
						{
							Name = y.Item.Name,
							Price = y.Item.Price,
							Quantity = y.Quantity
						}).ToArray(),
						TotalPrice = x.TotalPrice
					}).OrderByDescending(p => p.TotalPrice).ThenBy(p => p.Items.Length).ToArray(),
					TotalMade = c.Orders.Where(x => x.Type.ToString() == orderType)
					.Sum(t => t.TotalPrice)


				}).FirstOrDefault();

			return JsonConvert.SerializeObject(orders, Newtonsoft.Json.Formatting.Indented);
		}

		public static string ExportCategoryStatistics(FastFoodDbContext context, string categoriesString)
		{
			// which receives a string of comma - separated category names.
			// Export the categories: for each category, export its most popular item.
			// The most popular item is the item from the category,
			// which made the most money in orders.
			// Sort the categories by the amount of money the most popular item made(descending),
			// then by the times the item was sold(descending).

			var categories = categoriesString.Split(",");

			var result = context.Categories.Where(c => categories.Any(x => x == c.Name))
				.Select(x => new XmlExportDto
				{
					Name = x.Name,
					MostPopularItem = x.Items.Select(y => new MostPopularItem
					{
						Name = y.Name,
						TimesSold = y.OrderItems.Sum(t => t.Quantity),
						TotalMade = y.OrderItems.Sum(t => t.Item.Price * t.Quantity),

					}).OrderByDescending(t => t.TotalMade)
					.ThenByDescending(t => t.TimesSold)
					.FirstOrDefault(),
					
				}).OrderByDescending(t => t.MostPopularItem.TimesSold)
					.ThenByDescending(t => t.MostPopularItem.TotalMade)
					.ToArray();

			return SerializeXml(result, "Categories");
			  
		}

		private static string SerializeXml<T>(T[] objects, string root)
		{
			var serializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(root));
			var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName() });

			var sb = new StringBuilder();

			serializer.Serialize(new StringWriter(sb), objects, namespaces);

			return sb.ToString().TrimEnd();
		}
	}
}