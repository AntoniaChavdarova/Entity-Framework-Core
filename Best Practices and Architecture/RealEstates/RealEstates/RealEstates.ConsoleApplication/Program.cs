using Microsoft.EntityFrameworkCore;
using RealEstates.Data;
using System;

namespace RealEstates.ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            RealEstateDbContext db = new RealEstateDbContext();
            db.Database.Migrate();





        }
    }
}
