using CarDealer.Data;
using CarDealer.Export;
using CarDealer.Import;
using CarDealer.Models;
using CarDealer.XML_Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext db = new CarDealerContext();

            //db.Database.EnsureDeleted();
            //db.Database.EnsureCreated();

            //var xml1 = File.ReadAllText("../../../Datasets/suppliers.xml");
            //var xml2 = File.ReadAllText("../../../Datasets/parts.xml");
            //var xml3 = File.ReadAllText("../../../Datasets/cars.xml");
            // var xml4 = File.ReadAllText("../../../Datasets/customers.xml");
            // var xml5 = File.ReadAllText("../../../Datasets/sales.xml");

            //Console.WriteLine(ImportSuppliers(db, xml1));
            //Console.WriteLine(ImportParts(db, xml2));
            // Console.WriteLine(ImportCars(db, xml3));
            //Console.WriteLine(ImportCustomers(db, xml4));
            // Console.WriteLine(ImportSales(db, xml5));


           string res = GetSalesWithAppliedDiscount(db);
           File.WriteAllText("../../../Results/sales-discounts.xml", res);

          


        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //Get all sales with information about the car, customer and price of the sale with and without discount.
            var result = context.Sales
                .Select(x => new ExportSaleDiscountDto
                {
                    Car = new ExportCarInfoDto
                    {
                        Make = x.Car.Make,
                        Model = x.Car.Model,
                        TraveledDistance = x.Car.TravelledDistance
                    },

                    Discount = x.Discount,
                    CustomerName = x.Customer.Name,
                    Price = x.Car.PartCars.Sum(y => y.Part.Price),
                    PriceWithDiscount = x.Car.PartCars.Sum(y => y.Part.Price) -
                    x.Car.PartCars.Sum(y => y.Part.Price) * x.Discount / 100

                }).ToArray();

            return XmlConverter.Serialize(result, "sales");
        }


        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context
             .Customers
             .Select(x => new ExportTotalSaleDto()
             {
                 FullName = x.Name,
                 BoughtCars = x.Sales.Count,
                 SpentMoney = x.Sales.Sum(s => s.Car.PartCars.Sum(p => p.Part.Price))
             })
             .Where(c => c.BoughtCars >= 1)
             .OrderByDescending(c => c.SpentMoney)
             .ToArray();



            return XmlConverter.Serialize(customers, "customers");

        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsParts = context.Cars
                .Select(x => new ExportCarswithPartDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TraveledDistance = x.TravelledDistance,
                    Parts = x.PartCars.Select(pc => new ExportPartInfoDto
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    }).OrderByDescending(x => x.Price).ToArray()
                }).OrderByDescending(x => x.TraveledDistance).ThenBy(x => x.Model).Take(5).ToArray();

            return XmlConverter.Serialize(carsParts, "cars");

        }
       public static string GetLocalSuppliers(CarDealerContext context)
       {
            var suppliers = context.Suppliers.Where(x => x.IsImporter == false)
                .Select(x => new ExportLocalSuppliersDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    PartsCount = x.Parts.Count

                }).ToArray();

            return XmlConverter.Serialize(suppliers, "suppliers");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars.Where(x => x.Make == "BMW")
                .Select(x => new ExportCarBmxDto
                {
                    Id = x.Id,
                    Model = x.Model,
                    TraveledDistance = x.TravelledDistance

                }).OrderBy(x => x.Model).ThenByDescending(x => x.TraveledDistance).ToArray();

            return XmlConverter.Serialize(cars, "cars");
        }
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars.Where(x => x.TravelledDistance > 2000000)
                .Select(x => new CarsWithDistanceDto
                {
                    Make = x.Make,
                    Model = x.Model,
                    TraveledDistance = x.TravelledDistance

                }).OrderBy(x => x.Make).ThenBy(x => x.Model).Take(10).ToArray();

            return XmlConverter.Serialize(cars, "cars");
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var saleDto = XmlConverter.Deserializer<ImportSaleDto>(inputXml, "Sales");

            var sales = saleDto.Where(x => context.Cars.Any(c => c.Id == x.CardId))
                .Select(x => new Sale
                {
                    CarId = x.CardId,
                    CustomerId = x.CustomerId,
                    Discount = x.Discount
                }).ToArray();


            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Length}";

        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Parts";

            var result = XmlConverter.Deserializer<ImportPartDto>(inputXml, rootElement);

            var parts = result
                .Where(x => context.Suppliers.Any(s => s.Id == x.SupplierId))
                .Select(x => new Part
                {
                    Name = x.Name,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SupplierId = x.SupplierId

                }).ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";

        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Suppliers";

            var result = XmlConverter.Deserializer<ImportSupplierDto>(inputXml, rootElement);

            var suppliers = result
                .Select(x => new Supplier
                {
                    Name = x.Name,
                    IsImporter = x.IsImporter
                }).ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";

        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {

            const string rootElement = "Cars";

            var carDtos = XmlConverter.Deserializer<ImportCarDto>(inputXml, rootElement);

            var cars = new List<Car>();

            foreach (var carDto in carDtos)
            {
                var uniqueParts = carDto.Parts.Select(x => x.Id).Distinct().ToArray();
                var realParts = uniqueParts.Where(id => context.Parts.Any(i => i.Id == id)).ToArray();

                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TravelledDistance = carDto.TraveledDistance,
                    PartCars = realParts.Select(p => new PartCar
                    {
                        PartId = p
                    }).ToArray()

                };

                cars.Add(car);
            }


            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";

        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string rootElement = "Customers";

            var customerDto = XmlConverter.Deserializer<ImportCustomerDto>(inputXml, rootElement);

            var customers = customerDto.Select(x => new Customer
            {
                Name = x.Name,
                BirthDate = DateTime.Parse(x.BirthDate),
                IsYoungDriver = x.IsYoungDriver
            })
                .ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }
    }
}