namespace BookShop.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using BookShop.Data.Models.Enums;
    using BookShop.DataProcessor.ExportDto;
    using Data;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportMostCraziestAuthors(BookShopContext context)
        {
            var authors = context.Authors.Select(x => new
            {
                AuthorName = x.FirstName + " " + x.LastName,
                Books = x.AuthorsBooks
                .OrderByDescending(p => p.Book.Price) //zashtoto posle price e string !!
                .Select(y => new
                {
                    BookName = y.Book.Name,
                    BookPrice = y.Book.Price.ToString("f2")
                })
                  .ToArray()


            })
                .ToArray()
                .OrderByDescending(b => b.Books.Length)
                .ThenBy(b => b.AuthorName)
                .ToArray(); ;

            return JsonConvert.SerializeObject(authors, Formatting.Indented);
        }

        public static string ExportOldestBooks(BookShopContext context, DateTime date)
        {
            var books = context.Books
                 .Where(y => y.PublishedOn.Date < date && y.Genre == Genre.Science)
                  .OrderByDescending(x => x.Pages)
                 .ThenByDescending(x => x.PublishedOn)
                 .Select(y => new BookExportDto
                 {
                     BookName = y.Name,
                     Date = y.PublishedOn.ToString("d", CultureInfo.InvariantCulture),
                     Pages = y.Pages

                 }).Take(10)
                 .ToArray();

            return SerializeXml(books , "Books");
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