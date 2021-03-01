using ProductShop.Data;
using ProductShop.Import;
using ProductShop.XML_Helper;
using System.IO;
using System;
using System.Linq;
using ProductShop.Models;
using ProductShop.Export;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext db = new ProductShopContext();
            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var userXml = File.ReadAllText("../../../Datasets/categories-products.xml");
          // Console.WriteLine(ImportCategoryProducts(db , userXml

            string res = GetUsersWithProducts(db);

            File.WriteAllText("../../../Results/users-and-products.xml", res);
           
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //Select users who have at least 1 sold product. Order them by the number of sold products(from highest to lowest). Select only their first and last name, age, count of sold products and for each product -name and price sorted by price(descending).Take top 10 records.

            var users = context.Users
                
                .Where(x => x.ProductsSold.Any())
                
                .Select(x => new ExportUserProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProduct = new SoldProductInfoDto
                    {
                        Count = x.ProductsSold.Count,
                        Products = x.ProductsSold.Select(ps => new SoldProductDto
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .OrderByDescending(x => x.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(x => x.SoldProduct.Count)
                .Take(10).ToArray();

            var result = new ExportUserCountDto
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                Users = users
            };

            return XmlConverter.Serialize(result, "Users");
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //Get all categories.For each category select its name, the number of products, the average price of those products and the total revenue(total price sum) of those products(regardless if they have a buyer or not). Order them by the number of products(descending) then by total revenue.

            var categories = context.Categories
                .Select(x => new ExportCategoryByProductDto
                {
                    Name = x.Name,
                    Count = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = x.CategoryProducts.Sum(cp => cp.Product.Price)
                }).OrderByDescending(x => x.Count).ThenBy(x => x.TotalRevenue).ToArray();

            return XmlConverter.Serialize(categories, "Categories");

        }

       public static string GetSoldProducts(ProductShopContext context)
        {
            //Get all users who have at least 1 sold item. Order them by last name, then by first name. Select the person's first and last name. For each of the sold products, select the product's name and price. Take top 5 records.

            var users = context.Users.Where(x => x.ProductsSold.Any())
                .Select(x => new UserSoldProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    SoldProducts = x.ProductsSold
                   .Select(s => new UserProductDto
                    {
                        Name = s.Name,
                        Price = s.Price

                    }).ToArray()
                }).OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Take(5).ToArray();

            return XmlConverter.Serialize(users, "Users");
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            const string rootElement = "Products";

            var product = context.Products.Where(x => x.Price >= 500 && x.Price <= 1000)
                .Select(p => new ExportProductInfoDto
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                }).OrderBy(x => x.Price)
                .Take(10)
                .ToList();

            var result = XmlConverter.Serialize(product, rootElement);

            return result;

        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "CategoryProducts";

            var productCatElement = XmlConverter.Deserializer<ImportCategoryProductDto>(inputXml, rootElement);

            var catProducts = productCatElement
                .Where(x => context.Categories.Any(c => c.Id == x.CategoryId) && context.Products.Any(p => p.Id == x.ProductId))
                .Select(cp => new CategoryProduct
                {
                    CategoryId = cp.CategoryId,
                    ProductId = cp.ProductId
                }).ToArray();

            context.CategoryProducts.AddRange(catProducts);
            context.SaveChanges();

            return $"Successfully imported {catProducts.Length}";

        }


        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Categories";

            var categoryProduct = XmlConverter.Deserializer<ImportCategorieDto>(inputXml, rootElement);

            var categories = categoryProduct
                .Where(x => x.Name != null)
                .Select(c => new Category
                {
                    Name = c.Name
                }).ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            const string rootElement = "Products";

            var productResult = XmlConverter.Deserializer<ImportProductDto>(inputXml, rootElement);

            var products = productResult
                .Select(p => new Product
                {
                    Name = p.Name,
                    Price = p.Price,
                    SellerId = p.SellerId,
                    BuyerId = p.BuyerId

                }).ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        public static string ImportUsers(ProductShopContext context, string inputXml) 
        {
            const string rootElement = "Users";

            var userResult = XmlConverter.Deserializer<ImportUserDto>(inputXml, rootElement);

            var users = userResult
                .Select(u => new User
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age
                }).ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return  $"Successfully imported {users.Length}";
        }
    }
}