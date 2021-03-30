namespace TeisterMask.DataProcessor
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.DataProcessor.ExportDto;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        //Select the top 10 employees who have at least one task that its open date is after
        //    or equal to the given date with their tasks that meet
        //    the same requirement(to have their open date after or equal to the giver date).
        //    For each employee, export their username and their tasks.For each task, 
        //        export its name and open date (must be in format "d"),
        //    due date(must be in format "d"), label and execution type.
        //    Order the tasks by due date (descending), then by name (ascending).
        //        Order the employees by all tasks count (descending), then by username (ascending).
        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {

            var employees = context.Employees.Where(x => x.EmployeesTasks.Any(y => y.Task.OpenDate >= date))
                .Select(x => new
                {
                    Username = x.Username,
                    Tasks = x.EmployeesTasks
                    .Where(et => et.Task.OpenDate >= date)
                    .OrderByDescending(a => a.Task.DueDate).ThenBy(a => a.Task.Name).Select(y => new
                    {
                        TaskName = y.Task.Name,
                        OpenDate = y.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                        DueDate = y.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                        LabelType = y.Task.LabelType.ToString(),
                        ExecutionType = y.Task.ExecutionType.ToString()

                    }).ToArray()
                }).OrderByDescending(x => x.Tasks.Length).ThenBy(x => x.Username).Take(10).ToArray();

            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var projects = context.Projects.Where(x => x.Tasks.Any())
                .Select(x => new ExportProject
                {
                    Count = x.Tasks.Count,
                    ProjectName = x.Name,
                    HasEndDate = x.DueDate.HasValue ? "Yes" : "No",
                    Tasks = x.Tasks.Select(y => new TaskExport
                    {
                        Name = y.Name,
                        Label = y.LabelType.ToString()
                    }).OrderBy(a => a.Name).ToArray()
                }).OrderByDescending(t => t.Tasks.Length).ThenBy(t => t.ProjectName).ToArray();

            return SerializeXml(projects, "Projects");
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