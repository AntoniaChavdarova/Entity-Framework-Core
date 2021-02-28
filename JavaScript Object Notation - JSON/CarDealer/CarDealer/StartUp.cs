using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Models;
using Newtonsoft.Json;

namespace CarDealer
{
    public class StartUp
    {
        private static string ResultsDirectoryPath = "../../../Datasets/Results";
        public static void Main(string[] args)
        {
            
            CarDealerContext db = new CarDealerContext();


            //string inputJson = File.ReadAllText("../../../Datasets/sales.json");

            // Console.WriteLine(ImportSales(db, inputJson) ); 

            
            string json = GetSalesWithAppliedDiscount(db);

            EnsureDirectoryExists();

            File.WriteAllText(ResultsDirectoryPath + "/sales-discounts.json", json);

        }

        //Get first 10 sales with information about the car, customer and price of the sale with and without discount. Export the list of sales to JSON in the format provided below.


        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales.Select(x => new
            {
                car = new
                {
                    Make = x.Car.Make,
                    Model = x.Car.Model,
                    TravelledDistance = x.Car.TravelledDistance
                },

               
                    customerName = x.Customer.Name,
                    Discount = x.Discount,
                    Price = x.Car.PartCars.Sum(y => y.Part.Price),
                    priceWithDiscount = x.Car.PartCars.Sum(pc => pc.Part.Price) -
                                            x.Car.PartCars.Sum(pc => pc.Part.Price) * x.Discount / 100


            }).Take(10).ToList();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);

        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var users = context.Customers.Where(x => x.Sales.Any(s => s.CustomerId != null))
                .Select(x => new
                {
                    fullName = x.Name,
                    boughtCars = x.Sales.Count,
                    spentMoney = x.Sales
                   .Sum(s => s.Car.PartCars.Sum(y =>( y.Part.Price)))
                    
                })
                .ToList();

          
            return JsonConvert.SerializeObject(users, Formatting.Indented);

        }
        private static void ImportPartCars(CarDealerContext context)
        {
            int carsCount = context
                .Cars
                .Count();
            int partsCount = context.Parts.Count();

            var partCars = new List<PartCar>();

            for (int i = 1; i <= carsCount; i++)
            {
                var partCar = new PartCar();

                partCar.CarId = i;

                partCar.PartId = new Random().Next(1, partsCount);

                partCars.Add(partCar);
            }

            context.PartCars.AddRange(partCars);

            context.SaveChanges();
            Console.WriteLine($"Successfully added {partCars.Count()} partCars!");
        }
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance
                    },
                    parts = c.PartCars.Select(pc => new
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("f2")
                    }).ToList()
                }).ToList();
  
            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers.Where(x => x.IsImporter == false)
                .Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count
                }).ToList();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars.Where(x => x.Make == "Toyota")
                .Select(x => new
                {
                    Id = x.Id,
                    Make = x.Make,
                    Model = x.Model,
                    TravelledDistance = x.TravelledDistance

                })
                .OrderBy(x => x.Model).ThenByDescending(x => x.TravelledDistance).ToList();

            var json = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return json;
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(x => x.BirthDate)
                .ThenBy(x => x.IsYoungDriver)
                .Select(x => new
                {
                    Name = x.Name,
                    BirthDate = x.BirthDate.ToString("dd/MM/yyyy"),
                    IsYoungDriver = x.IsYoungDriver

                }).ToList();

            string result = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return result;
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            Sale[] sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {

           Customer[] customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            Car[] cars = JsonConvert.DeserializeObject<Car[]>(inputJson);

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Length}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            Part[] parts = JsonConvert.DeserializeObject<Part[]>(inputJson);

            var suppliers = context.Suppliers.Select(s => s.Id);
            parts = parts.Where(p => suppliers.Any(s => s == p.SupplierId)).ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}.";

        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            Supplier[] suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

           return $"Successfully imported {suppliers.Length}.";
        }

        private static void EnsureDirectoryExists()
        {
            if (!Directory.Exists(ResultsDirectoryPath))
            {
                Directory.CreateDirectory(ResultsDirectoryPath);
            }
        }

        private static void ResetDatabase(CarDealerContext db)
        {
            db.Database.EnsureDeleted();
            Console.WriteLine("Database was successfully deleted!");

            db.Database.EnsureCreated();
            Console.WriteLine("Database was successfully created!");
        }
    }
}