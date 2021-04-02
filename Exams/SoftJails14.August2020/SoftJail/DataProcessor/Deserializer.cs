namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using System.Linq;
    using System.Globalization;
    using SoftJail.Data.Models.Enums;

    public class Deserializer
    {
        private const string error = "Invalid Data";
        private const string success = "Imported {0} with {1} cells";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var departmentsDto = JsonConvert.DeserializeObject<ImportDepartmentDto[]>(jsonString);
            var sb = new StringBuilder();

            var listDepartments = new List<Department>();

            foreach (var departmentDto in departmentsDto)
            {
                if (!IsValid(departmentDto) || !departmentDto.Cells.All(IsValid) || !departmentDto.Cells.Any()) 
                {
                    sb.AppendLine(error);
                    continue;
                }

                var department = new Department
                {
                    Name = departmentDto.Name,
                    Cells = departmentDto.Cells.Select(x => new Cell
                    {
                        CellNumber = x.CellNumber,
                        HasWindow = x.HasWindow
                    }).
                    ToList()
                };

                listDepartments.Add(department);
                sb.AppendLine(string.Format(success, department.Name, department.Cells.Count()));

            }

            context.Departments.AddRange(listDepartments);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var prisonerDtos = JsonConvert.DeserializeObject<ImportPrisonerDto[]>(jsonString);
            var sb = new StringBuilder();
            var prisonersList = new List<Prisoner>();

            foreach (var prisonerDto in prisonerDtos)
            {
                if(!IsValid(prisonerDto) || !prisonerDto.Mails.All(IsValid))
                {
                    sb.AppendLine(error);
                    continue;
                }

                bool isValidDate = DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture
                    ,DateTimeStyles.None , out DateTime releaseDate);
                var dateIn = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    ReleaseDate = isValidDate ? (DateTime?)releaseDate : null,
                    IncarcerationDate = dateIn,
                    Bail = prisonerDto.Bail,
                    CellId = prisonerDto.CellId,
                    Mails = prisonerDto.Mails.Select(x => new Mail
                    {
                        Description = x.Description,
                        Sender = x.Sender,
                        Address = x.Address
                    }).ToArray()

                };

                prisonersList.Add(prisoner);
                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");

            }
            context.AddRange(prisonersList);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var officerDtos = DeserializeObject<ImportOfficerXmlDto>("Officers", xmlString);
            var sb = new StringBuilder();
            var officerList = new List<Officer>();

            foreach (var officerDto in officerDtos)
            {
                if (!IsValid(officerDto))
                {
                    sb.AppendLine(error);
                    continue;
                }

                var officer = new Officer
                {
                    FullName = officerDto.Name,
                    Salary = officerDto.Money,
                    Position = Enum.Parse<Position>(officerDto.Position),
                    Weapon = Enum.Parse<Weapon>(officerDto.Weapon),
                    DepartmentId = officerDto.DepartmentId,
                    OfficerPrisoners = officerDto.Prisoners.Select(x => new OfficerPrisoner
                    {
                        PrisonerId = x.Id
                    }).ToArray()
                };

                officerList.Add(officer);
                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");

            }

            context.AddRange(officerList);
            context.SaveChanges();
            return sb.ToString().TrimEnd();

        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }

        private static T[] DeserializeObject<T>(string rootElement, string xmlString)
        {
            var xmlSerializer = new XmlSerializer(typeof(T[]), new XmlRootAttribute(rootElement));
            var deserializedDtos = (T[])xmlSerializer.Deserialize(new StringReader(xmlString));
            return deserializedDtos;
        }
    }
}