﻿using MiniORM.App.Data;
using MiniORM.App.Data.Entities;
using System;
using System.Linq;

namespace MiniORM.App
{
    class StartUp
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=MiniORM;Integrated Security =true";

            var context = new SoftUniDbContext(connectionString);

            context.Employees.Add(new Employee
            {
                FirstName = "Ivan",
                LastName = "Ivanov",
                DepartmentId = context.Departments.First().Id,
                IsEmployed = true,
            }) ;


           var employee = context.Employees.Last();
           employee.FirstName = "Modified";

            context.SaveChanges();

            

        }
    }
}
