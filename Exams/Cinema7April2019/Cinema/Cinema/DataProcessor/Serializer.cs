namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            //The given method in the project skeleton receives movie rating.
            //Export all movies which have rating more or equal to the given and have at least one projection
            //with sold tickets. For each movie, export its name, rating formatted to the second digit,
            //total incomes formatted same way and customers.
            //For each customer, export its first name, last name and balance formatted to the second digit.
            //Order the customers by balance (descending), then by first name (ascending)
            //and last name (ascending). Take first 10 records and order the movies by rating (descending),
            //then by total incomes (descending).

            var movies = context.Movies.Where(x => x.Rating >= rating && x.Projections.Any(a => a.Tickets.Count > 0))
                .ToList()
                .OrderByDescending(x => x.Rating)
                .ThenByDescending(x => x.Projections
                    .Sum(y => y.Tickets
                        .Sum(t => t.Price)))
                .Select(x => new
                {
                    MovieName = x.Title,
                    Rating = x.Rating.ToString("f2"),
                    TotalIncomes = x.Projections
                    .Sum(a => a.Tickets.Sum(y => y.Price)).ToString("f2"),
                    Customers =
                    x.Projections.SelectMany(y => y.Tickets.Select(b => new
                    {
                        FirstName = b.Customer.FirstName,
                        LastName = b.Customer.LastName,
                        Balance = b.Customer.Balance.ToString("f2")
                    })).ToArray()

                }).Take(10).ToArray();

            return JsonConvert.SerializeObject(movies, Newtonsoft.Json.Formatting.Indented);
        }


        public static string ExportTopCustomers(CinemaContext context, int age)
        {

            var customers = context.Customers.Where(x => x.Age >= age)
                .OrderByDescending(c => c.Tickets.Sum(t => t.Price))
                .Select(x => new CustomerExportDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SpentMoney = x.Tickets.Sum(t => t.Price).ToString("f2"),
                    SpentTime = TimeSpan
                        .FromMilliseconds(x.Tickets.Sum(t => t.Projection.Movie.Duration.TotalMilliseconds))
                        .ToString(@"hh\:mm\:ss")
                })
                .Take(10)
                .ToArray();

            return SerializeXml(customers, "Customers");
                
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