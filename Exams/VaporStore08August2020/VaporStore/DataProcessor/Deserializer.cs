namespace VaporStore.DataProcessor
{
	using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Import;
    using VaporStore.ImportResults;

    public static class Deserializer
	{
		public const string Error = "Invalid Data";


		public static string ImportGames(VaporStoreDbContext context, string jsonString)
		{
			var gamesDtos = JsonConvert.DeserializeObject<GameDto[]>(jsonString);
			var sb = new StringBuilder();
		

            foreach (var gameDto in gamesDtos)
            {
				if(!IsValid(gameDto) || !gameDto.Tags.Any() )
                {
					sb.AppendLine(Error);
					continue;
                }

				var date = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

				var developer = context.Developers.FirstOrDefault(x => x.Name == gameDto.Developer);
				if(developer == null)
                {
					developer = new Developer
					{
						Name = gameDto.Developer
					};

				}
				
				var genre = context.Genres.FirstOrDefault(x => x.Name == gameDto.Genre);
				
				if (genre == null)
				{
					 genre = new Genre
					{
						Name = gameDto.Genre
					};

				}
			
				var game = new Game
				{
					Name = gameDto.Name,
					Price = gameDto.Price,
					ReleaseDate = date,
					Developer = developer,
					Genre = genre,
				};

				foreach (var t in gameDto.Tags)
				{
					var tag = context.Tags.FirstOrDefault(x => x.Name == t)
						?? new Tag { Name = t };

					game.GameTags.Add(new GameTag { Tag = tag });
					
				}

				context.Games.Add(game);
				context.SaveChanges();
				sb.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags"!);
			}

			
			return sb.ToString().TrimEnd();
		}

		public static string ImportUsers(VaporStoreDbContext context, string jsonString)
		{
			var usersDtos = JsonConvert.DeserializeObject<UserDto[]>(jsonString);
			var sb = new StringBuilder();
			var list = new List<User>();

            foreach (var userDto in usersDtos)
            {

                if (!IsValid(userDto) || !userDto.Cards.Any(IsValid))
                {
					sb.AppendLine(Error);
					continue;
                }

				var user = new User
				{
					FullName = userDto.FullName,
					Username = userDto.Username,
					Email = userDto.Email,
					Age = userDto.Age,
					Cards = userDto.Cards.Select(x => new Card
					{
						Number = x.Number,
						Cvc = x.Cvc,
						Type = Enum.Parse<CardType>(x.Type)
					}).ToArray()

				};

				list.Add(user);
				sb.AppendLine($"Imported {user.Username} with {user.Cards.Count} cards");
            }

			context.Users.AddRange(list);
			context.SaveChanges();
			return sb.ToString().TrimEnd();

		}

		public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
		{
			var purchaseDtos = DeserializeObject<PurchaseDto>("Purchases", xmlString);
			var sb = new StringBuilder();
			var list = new List<Purchase>();

            foreach (var purchaseDto in purchaseDtos)
            {
                if (!IsValid(purchaseDto))
                {
					sb.AppendLine(Error);
					continue;
                }

				var date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);

				var card = context.Cards.FirstOrDefault(a => a.Number == purchaseDto.Card);
				
				var game = context.Games.FirstOrDefault(a => a.Name == purchaseDto.Title);

				

				var purchase = new Purchase
				{
					Game = game,
					Type = Enum.Parse<PurchaseType>(purchaseDto.Type),
					ProductKey = purchaseDto.ProductKey,
					Card = card,
					Date = date
				};

				list.Add(purchase);
				sb.AppendLine($"Imported {purchase.Game.Name} for {purchase.Card.User.Username}");
			}

			context.Purchases.AddRange(list);
			context.SaveChanges();
			return sb.ToString().TrimEnd();
		}

		private static bool IsValid(object dto)
		{
			var validationContext = new ValidationContext(dto);
			var validationResult = new List<ValidationResult>();

			return Validator.TryValidateObject(dto, validationContext, validationResult, true);
		}

		private static T[] DeserializeObject<T>(string rootElement, string xmlString)
		{
			var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootElement));
			var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));
			return deserializedDtos;
		}

	}
}