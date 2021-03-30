namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    using Data;
    using System.Xml.Serialization;
    using System.IO;
    using TeisterMask.DataProcessor.ImportDto;
    using System.Text;
    using TeisterMask.Data.Models;
    using System.Linq;
    using System.Globalization;
    using TeisterMask.Data.Models.Enums;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            var projectsDtos = DeserializeObject<ProjectImportDto>("Projects", xmlString);
            var sb = new StringBuilder();
            var list = new List<Project>();

            foreach (var projectDto in projectsDtos)
            {
                
                if (!IsValid(projectDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
              
              var IsValiddateDue = DateTime.TryParseExact(projectDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate);
              var dateOpen = DateTime.ParseExact(projectDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                
                    var project = new Project
                {
                    Name = projectDto.Name,
                    OpenDate = dateOpen,
                    DueDate = IsValiddateDue ? (DateTime?)dueDate : null,
                   
                };

                foreach (var taskDto in projectDto.Tasks)
                {

                    if (!IsValid(taskDto))
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;

                    }

                    Task task = new Task
                    {
                        Name = taskDto.Name,
                        OpenDate = DateTime.ParseExact(taskDto.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        DueDate = DateTime.ParseExact(taskDto.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        ExecutionType = Enum.Parse<ExecutionType>(taskDto.ExecutionType),
                        LabelType = Enum.Parse<LabelType>(taskDto.LabelType),

                    };


                    //task open date is before project open date
                        if (project.OpenDate > task.OpenDate)
                        {
                            sb.AppendLine(ErrorMessage);

                            continue;
                        }

                   // or task due date is after project due date
                    if (project.DueDate < task.DueDate)
                    {
                        sb.AppendLine(ErrorMessage);

                        continue;
                    }

                    project.Tasks.Add(task);
                }
               

                list.Add(project);
                sb.AppendLine(String.Format(SuccessfullyImportedProject , project.Name , project.Tasks.Count));
            }

            context.AddRange(list);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            var employeesDtos = JsonConvert.DeserializeObject<EmployeeImportDto[]>(jsonString);
            var sb = new StringBuilder();
            var list = new List<Employee>();

            foreach (var employeeDto in employeesDtos)
            {
                if (!IsValid(employeeDto))
                {
                    sb.AppendLine(ErrorMessage);

                    continue;
                }

                Employee employee = new Employee
                {
                    Username = employeeDto.Username,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone
                };

                foreach (int taskid in employeeDto.Tasks.Distinct())
                {
                    Task task = context.Tasks
                     .FirstOrDefault(t => t.Id == taskid);

                    if (task == null)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employee.EmployeesTasks.Add(new EmployeeTask()
                    {
                        Employee = employee,
                        Task = task
                    });
                }
                list.Add(employee);
                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employee.Username,
                    employee.EmployeesTasks.Count));
            }

            context.Employees.AddRange(list);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }

        private static T[] DeserializeObject<T>(string rootElement, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootElement));
            var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));
            return deserializedDtos;
        }
    }
}