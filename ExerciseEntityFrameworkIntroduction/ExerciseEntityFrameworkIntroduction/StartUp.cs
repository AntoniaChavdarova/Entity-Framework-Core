using SoftUni.Data;
using SoftUni.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

           
            var res = RemoveTown(context);
            Console.WriteLine(res);
        }

        public static string RemoveTown(SoftUniContext context)
        {
                var town = context.Towns
                    .First(t => t.Name == "Seattle");

                var addressesToDel = context
                    .Addresses
                    .Where(s => s.TownId == town.TownId);
                int addressesCount = addressesToDel.Count();

                var employees = context.Employees
                    .Where(e => addressesToDel.Any(a => a.AddressId == e.AddressId));

                foreach (var e in employees)
                {
                    e.AddressId = null;
                }

                foreach (var a in addressesToDel)
                {
                    context.Addresses.Remove(a);
                }

                context.Towns.Remove(town);

                context.SaveChanges();

                return $"{addressesCount} addresses in {town.Name} were deleted";
          
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder sb = new StringBuilder();
            var employeesProjectsToDelete = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2);

            var project = context.Projects
                .Where(p => p.ProjectId == 2)
                .Single();

            foreach (var ep in employeesProjectsToDelete)
            {
                context.EmployeesProjects.Remove(ep);
            }

            context.Projects.Remove(project);

            context.SaveChanges();

            context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList()
                .ForEach(p => sb.AppendLine(p));

            return sb.ToString().Trim();
        }
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(x => x.FirstName.StartsWith("Sa") ||  x.FirstName.StartsWith("SA") || x.FirstName.StartsWith("sa"))
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    x.Salary
                }).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }

            return sb.ToString().TrimEnd();
        }

        public static string IncreaseSalaries(SoftUniContext context)
        {
            var sb = new StringBuilder();

            IQueryable<Employee> employeesForIncreasing = context.Employees
                .Where(x => x.Department.Name == "Engineering" || 
                x.Department.Name == "Tool Design"
               || x.Department.Name == "Marketing" || x.Department.Name == "Information Services");

            foreach (Employee e in employeesForIncreasing)
            {
                e.Salary += e.Salary * 0.12m;
            }

            context.SaveChanges();

            var employessResult = employeesForIncreasing
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.Salary

                })
                .OrderBy(x => x.FirstName).ThenBy(x => x.LastName)
                .ToList();

            foreach (var er in employessResult)
            {
                sb.AppendLine($"{er.FirstName} {er.LastName} (${er.Salary:f2})");
            }
               
            return sb.ToString().TrimEnd();

        }

        public static string GetLatestProjects(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .Select(p => new
                {
                    ProjectName = p.Name,
                    Description = p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture)
                })
                .OrderBy(p => p.ProjectName)
                .ToList();

            foreach (var p in projects)
            {
                sb.AppendLine($"{p.ProjectName}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.StartDate}");
            }
           
            return sb.ToString().TrimEnd();

        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var departments = context.Departments
                 .Where(x => x.Employees.Count() > 5)
                 .OrderBy(x => x.Employees.Count()).ThenBy(x => x.Name)
                  .Select(x => new
                  {
                     DepName = x.Name,
                      Count = x.Employees.Count(),
                      ManagerName = x.Manager.FirstName,
                      ManagerLastName =  x.Manager.LastName,
                      DepEmployees = x.Employees
                      .Select(e => new
                      {
                          EFirsName = e.FirstName,
                          ELastName = e.LastName,
                          EJobTitle = e.JobTitle

                      }).OrderBy(e => e.EFirsName).ThenBy(e => e.ELastName).ToList()
                  }) 
                 
                .ToList();



            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepName} - {d.ManagerName} {d.ManagerLastName}");
                

                foreach (var p in d.DepEmployees)
                {
                    sb.AppendLine($"{p.EFirsName} {p.ELastName} - {p.EJobTitle}");
                }
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employee = context.Employees
                .Where(x => x.EmployeeId == 147)
                .Select(x => new
                {

                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Project = x.EmployeesProjects.Select(e => new
                    {
                        e.Project.Name
                    }).OrderBy(x => x.Name).ToList()
                })
                .ToList();

            foreach (var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");

                foreach (var p in e.Project)
                {
                    sb.AppendLine($"{p.Name}");
                }
            }
           
            return sb.ToString().TrimEnd();
        }
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var sb = new StringBuilder();
            var addresses = context.Addresses
               .OrderByDescending(a => a.Employees.Count)
               .ThenBy(a => a.Town.Name)
               .ThenBy(a => a.AddressText)
               .Take(10)
               .Select(a => new
               {
                   AddressText = a.AddressText,
                   TownName = a.Town.Name,
                   EmployeeCount = a.Employees.Count
               })
               .ToList();

            foreach (var a in addresses)
            {
                sb.AppendLine($" {a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employess = context.Employees
                .Where(x => x.EmployeesProjects
                .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Take(10)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    ManagerFirstName = x.Manager.FirstName,
                    ManagerLastName = x.Manager.LastName,
                    Project = x.EmployeesProjects
                     .Select(ep => new
                     {
                         ProjectName = ep.Project.Name,
                         StartDate = ep.Project.StartDate
                         .ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                         EndDate = ep.Project.EndDate.HasValue ?
                         ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) : "not finished"
                     }).ToList()
                }).ToList();
            

            foreach (var item in employess)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} - Manager: {item.ManagerFirstName} {item.ManagerLastName}");

                foreach (var p in item.Project)
                {
                    sb.AppendLine($"--{p.ProjectName} - {p.StartDate} - {p.EndDate}");
                }
            }

            return sb.ToString().TrimEnd();
        }
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            var sb = new StringBuilder();

            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employeeNakov = context.Employees
                 .First(x => x.LastName == "Nakov");

            context.Addresses.Add(address);
            employeeNakov.Address = address;
            context.SaveChanges();

            var addresses = context.
                Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToList();

            foreach (var item in addresses)
            {
                sb.AppendLine($"{item}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employess = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.Department,
                    e.Salary
                })
                .OrderBy(e => e.Salary).ThenByDescending(e => e.FirstName)
                .ToList();

            foreach (var item in employess)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} from {item.Department.Name} - ${item.Salary:f2}");
            }

            return sb.ToString().TrimEnd();

        }
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employess = context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .OrderBy(e => e.EmployeeId)
                .ToList();

            foreach (var item in employess)
            {
                sb.AppendLine($"{item.FirstName} {item.LastName} {item.MiddleName} {item.JobTitle} {item.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var sb = new StringBuilder();

            var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToList();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
                
        }
    }
}
                                            