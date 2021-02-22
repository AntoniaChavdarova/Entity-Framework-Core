namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            // DbInitializer.ResetDatabase(db);

           
          

        }

        //Problem 2
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            
            var books = context
                .Books
                .AsEnumerable()
                .Where(x => x.AgeRestriction.ToString().ToLower() == command.ToLower())
                .Select(x => x.Title)
                .OrderBy(bt => bt)
                .ToList();



            return string.Join(Environment.NewLine, books);
        }

        //Problem 3
        public static string GetGoldenBooks(BookShopContext context)
        {
            var goldenTitles = context
                .Books
                .Where(b => b.Copies < 5000 && b.EditionType == EditionType.Gold)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, goldenTitles);
        }

        //Problem 4
        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(b => b.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(x => x.Price)
                .ToList();

            foreach (var b in books)
            {
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //Problem 5
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context
                .Books
                .Where(x => x.ReleaseDate.Value.Year != year)
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            return string.Join(Environment.NewLine, books);

        }

        //Problem 6
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            List<string> categories = input
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower())
                .ToList();

            List<string> bookTitles = new List<string>();

            foreach (var category in categories)
            {

                List<string> currentCategoryBooks = context.
                    Books
                    .Where(b => b.BookCategories.Any(bc => bc.Category.Name.ToLower() == category))
                    .Select(b => b.Title)
                    .ToList();
                bookTitles.AddRange(currentCategoryBooks);
            }

            return string.Join(Environment.NewLine, bookTitles.OrderBy(b => b));
        }

        //Problem 7
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);
            StringBuilder sb = new StringBuilder();

            var books = context
                .Books
                .Where(x => x.ReleaseDate.Value < dateTime)
                .OrderByDescending(x => x.ReleaseDate)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price
                })
                .ToList();

            foreach (var item in books)
            {
                sb.AppendLine($"{item.Title} - {item.EditionType} - ${item.Price:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            
            List<string> authors = context.Authors
               .Where(a => a.FirstName.EndsWith(input))
               .Select(a => a.FirstName + " " + a.LastName)
               .OrderBy(a => a)
               .ToList();

            return string.Join(Environment.NewLine, authors);

        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var titles = context
                .Books
                .Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(x => x.Title)
                .Select(x => x.Title)
                .ToList();

            return string.Join(Environment.NewLine, titles);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context
                .Books
                .Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(x => x.BookId)
                .Select(x => x.Title)
                .ToList();

            return string.Join(Environment.NewLine, books);
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var count = context
                .Books.Where(x => x.Title.Length > lengthCheck)
                .Count();

            return count;
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context
                .Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    CountCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.CountCopies)
                .ToList();

            foreach (var r in result)
            {
                sb.AppendLine($"{r.FullName} - {r.CountCopies}");
            }

            return sb.ToString().Trim();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var result = context
                .Categories
                .Select(b => new
                {
                    b.Name,
                    TotalProfit = b.CategoryBooks.Select(bk => new
                    {
                        Profit = bk.Book.Copies * bk.Book.Price,
                    }).Sum(x => x.Profit)
                }).OrderByDescending(bk => bk.TotalProfit).ThenBy(bk => bk.Name).ToList();

            foreach (var r in result)
            {
                sb.AppendLine($"{r.Name} ${r.TotalProfit:f2}");
            }

            return sb.ToString().Trim();
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var recentBooksByCategory = context.
                Categories
                .Select(x => new
                {
                    x.Name,
                    MostRecents = x.CategoryBooks
                    .OrderByDescending(cb => cb.Book.ReleaseDate)
                    .Take(3)
                    .Select(b => new
                    {
                        b.Book.Title,
                        Year = b.Book.ReleaseDate.Value.Year
                    })
                })
                .OrderBy(c => c.Name)
                .ToList();

            foreach (var category in recentBooksByCategory)
            {
                sb.AppendLine($"--{category.Name}");

                foreach (var book in category.MostRecents)
                {
                    sb.AppendLine($"{book.Title} ({book.Year})");
                }
            }

            return sb.ToString().Trim();


        }

      public static void IncreasePrices(BookShopContext context)
      {

            var booksToUpdate = context
                .Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToList();

            foreach (var book in booksToUpdate)
            {
                book.Price +=  5;
            }

            context.SaveChanges();
      }

        public static int RemoveBooks(BookShopContext context)
        {
            var categoryBooks = context.BooksCategories
                .Where(bc => bc.Book.Copies < 4200);

            context.BooksCategories.RemoveRange(categoryBooks);

            context.SaveChanges();

            var books = context.Books
                .Where(b => b.Copies < 4200);

            int count = books.Count();
            context.Books.RemoveRange(books);

            context.SaveChanges();
            return count;
        }

    }

 }



