namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Models.Enums;
    using VaporStore.DataProcessor.Dto.Export;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
			//			The given method in the project skeleton receives an array of genre names.
			//				Export all games in those genres, which have any purchases.
			//				For each genre, export its id, genre name, games and total players(total purchase count)
			//				.For each game, export its id, name, developer, tags (separated by ", ") 
			//				and total player count(purchase count).
			//				Order the games by player count(descending), then by game id(ascending).
			//Order the genres by total player count(descending), then by genre id(ascending)

			var genres = context.Genres.ToList().Where(x => genreNames.Contains(x.Name))
					.Select(y => new
					{
						Id = y.Id,
						Genre = y.Name,
						Games = y.Games.Select(x => new
						{
							Id = x.Id,
							Title = x.Name,
							Developer = x.Developer.Name,
							Tags = String.Join(", " , x.GameTags.Select(x => x.Tag.Name)),
							Players = x.Purchases.Count,
						}).Where(g => g.Players > 0)
						.OrderByDescending(x => x.Players).ThenBy(x => x.Id).ToArray(),
						TotalPlayers = y.Games.Sum(x => x.Purchases.Count)
					}).ToArray().OrderByDescending(x => x.Games.Sum(y => y.Players)).ThenBy(x => x.Id).ToArray();

			string res = JsonConvert.SerializeObject(genres, Newtonsoft.Json.Formatting.Indented);
			return res;
		}

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{

			//	Export all users who have any purchases. 
			//	For each user, export their username, purchases for that purchase type
			//	and total money spent for that purchase type.
			//		For each purchase, export its card number, CVC, date in the format
			//		"yyyy-MM-dd HH:mm"(make sure you use CultureInfo.InvariantCulture)
			//		and the game.For each game, export its title(name), genre and price.
			//		Order the users by total spent(descending),
			//		then by username(ascending).For each user, order the purchases by date(ascending).
			//		Do not export users, who don’t have any purchases.

			var purchase = context.Users.ToList()
				.Where(x => x.Cards.Any(c => c.Purchases.Any(p => p.Type.ToString() == storeType)))
				.Select(x => new XmlDto
                {
					UserName = x.Username,
					TotalSpent = x.Cards.Sum(x => 
					x.Purchases.Where(a => a.Type.ToString() == storeType)
					.Sum(a => a.Game.Price)),
					Purchases = x.Cards.SelectMany(p => p.Purchases)
					.Where(x => x.Type.ToString() == storeType)
					.Select(x => new PurchasesDto
                    {
						Card = x.Card.Number,
						Cvc = x.Card.Cvc,
						Date = x.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
						Game = new GameDto
                        {
							Genre = x.Game.Genre.Name,
							Title = x.Game.Name,
							Price = x.Game.Price
                        }

                    }).OrderBy(x => x.Date) //zashtoto e oburnata datata 
						.ToArray()
				}).OrderByDescending(x => x.TotalSpent).ThenBy(x => x.UserName).ToArray();

			return SerializeXml(purchase, "Users");
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