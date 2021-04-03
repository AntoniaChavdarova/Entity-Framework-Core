using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FastFood.Data;
using FastFood.DataProcessor.Dto.Import;
using FastFood.Models;
using FastFood.Models.Enums;
using Newtonsoft.Json;

namespace FastFood.DataProcessor
{
	public static class Deserializer
	{
		private const string FailureMessage = "Invalid data format.";
		private const string SuccessMessage = "Record {0} successfully imported.";

		public static string ImportEmployees(FastFoodDbContext context, string jsonString)
		{
			var employeesDtos = JsonConvert.DeserializeObject<EmployeeImport[]>(jsonString);
			var sb = new StringBuilder();

            foreach (var employeeDto in employeesDtos)
            {
                if (!IsValid(employeeDto))
                {
					sb.AppendLine(FailureMessage);
					continue;
                }

				var position = context.Positions.FirstOrDefault(x => x.Name == employeeDto.Position);
				if(position == null)
                {
					position = new Models.Position
					{
						Name = employeeDto.Position
					};
                }

				var emloyee = new Employee
				{
					Name = employeeDto.Name,
					Age = employeeDto.Age,
					Position = position

				};

				context.Employees.Add(emloyee);
				context.SaveChanges();
				sb.AppendLine(String.Format(SuccessMessage, emloyee.Name));
            }

			return sb.ToString().TrimEnd();
		}

		public static string ImportItems(FastFoodDbContext context, string jsonString)
		{
			var itemsDtos = JsonConvert.DeserializeObject<ItemsDto[]>(jsonString);
			var sb = new StringBuilder();

            foreach (var itemDto in itemsDtos)
			{
                //If an item with the same name already exists, ignore the entity and do not import it.
                //If an item’s category doesn’t exist, create it along with the item.

                if (!IsValid(itemDto))
                {
					sb.AppendLine(FailureMessage);
					continue;
				}

				var item = context.Items.FirstOrDefault(x => x.Name == itemDto.Name);
				if(item != null)
                {
					sb.AppendLine(FailureMessage);
					continue;
                }
				var category = context.Categories.FirstOrDefault(x => x.Name == itemDto.Category);
				if(category == null)
                {
					category = new Category
					{
						Name = itemDto.Category
					};
                }

				item = new Item
				{
					Name = itemDto.Name,
					Price = itemDto.Price,
					Category = category
					
				};

				context.Items.Add(item);
				context.SaveChanges();
				sb.AppendLine(String.Format(SuccessMessage, item.Name));
			}

			return sb.ToString().TrimEnd();
		}

		public static string ImportOrders(FastFoodDbContext context, string xmlString)
		{
			var ordersDtos = DeserializeObject<OrderXmlDto>("Orders", xmlString);
			var sb = new StringBuilder();

			foreach (var orderDto in ordersDtos)
			{
//				•	If the order’s employee doesn’t exist, do not import the order.
//•	If any of the order’s items do not exist, do not import the order.
//•	If there are any other validation errors(such as negative or non - zero price), proceed as described above.
//•	Every employee will have a unique name

				if (!IsValid(orderDto) || !orderDto.Items.Any(IsValid))
				{
					sb.AppendLine(FailureMessage);
					continue;
				}

				var date = DateTime.ParseExact(orderDto.DateTime, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
				var employee = context.Employees.FirstOrDefault(x => x.Name == orderDto.Employee);
				if(employee == null)
                {
					sb.AppendLine(FailureMessage);
					continue;
				}

                foreach (var items in orderDto.Items)
                {
				var item = context.Items.FirstOrDefault(x => x.Name == items.Name);

					if (item == null)
					{
						sb.AppendLine(FailureMessage);
						continue;
					}

                }

				var type = Enum.Parse<OrderType>(orderDto.Type);
				var order = new Order
				{
					Customer = orderDto.Customer,
					Employee = employee,
					DateTime = date,
					Type = type,
					OrderItems = orderDto.Items.Select(x => new OrderItem
					{
					    Item = context.Items.FirstOrDefault(it => it.Name == x.Name),
						Quantity = x.Quantity
					}).ToArray()
					
				};
			
				context.Orders.Add(order);
				context.SaveChanges();
				sb.AppendLine($"Order for {order.Customer} on {order.DateTime.ToString("dd/MM/yyyy HH:mm" , CultureInfo.InvariantCulture)} added");
			}
			return sb.ToString().TrimEnd();
		}
		private static T[] DeserializeObject<T>(string rootElement, string xmlString)
		{
			var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootElement));
			var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));
			return deserializedDtos;
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}
	}
}